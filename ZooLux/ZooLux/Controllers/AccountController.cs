using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using ZooLux.Models.Data;
using ZooLux.Models.ViewModels.Account;
using ZooLux.Models.ViewModels.Shop;

namespace ZooLux.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        // GET: account/create-account
        [ActionName("create-account")]
        [HttpGet]
        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        // POST: account/create-account/model
        [ActionName("create-account")]
        [HttpPost]
        public ActionResult CreateAccount(UserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateAccount", model);
            }

            if (!model.Password.Equals(model.ConfirmPassword))
            {
                ModelState.AddModelError("", "Password don't match!");
                return View("CreateAccount", model);
            }

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username)))
                {
                    ModelState.AddModelError("", $"Username {model.Username} is taken.");
                    model.Username = "";
                    return View("CreateAccount", model);
                }

                UserDTO userDTO = new UserDTO()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    EmailAdress = model.EmailAdress,
                    Username = model.Username,
                    Password = model.Password
                };

                db.Users.Add(userDTO);
                db.SaveChanges();

                int id = userDTO.Id;

                UserRoleDTO userRoleDTO = new UserRoleDTO()
                {
                    UserId = id,
                    RoleId = 2
                };

                db.UserRoles.Add(userRoleDTO);
                db.SaveChanges();
            }
            TempData["SM"] = "You are now registered and can login.";

            return RedirectToAction("Login");
        }

        // GET: Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            string userName = User.Identity.Name;

            if (!string.IsNullOrEmpty(userName))
            {
                return RedirectToAction("user-profile");
            }
            return View();
        }

        // POST: Account/Login/model
        [HttpPost]
        public ActionResult Login(LoginUserVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool isValid = false;

            using (Db db = new Db())
            {
                if (db.Users.Any(x => x.Username.Equals(model.Username) && x.Password.Equals(model.Password)))
                    isValid = true;

                if (!isValid)
                {
                    ModelState.AddModelError("", "Invalid username of password.");
                    return View(model);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
                    return Redirect(FormsAuthentication.GetRedirectUrl(model.Username, model.RememberMe));
                }
            }
        }

        // GET: /account/logout
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult UserNavPartial()
        {
            string userName = User.Identity.Name;

            UserNavPartialVM model = new UserNavPartialVM();

            using (Db db = new Db())
            {
                UserDTO dto = new UserDTO();
                dto = db.Users.FirstOrDefault(x => x.Username == userName);

                if (dto != null)
                {
                    model = new UserNavPartialVM()
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName
                    };
                }
            }
            return PartialView(model);
        }

        // GET: /account/user-profile
        [HttpGet]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            // Get the name of user
            string userName = User.Identity.Name;

            // Assign the model
            UserVM model;

            using (Db db = new Db())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.Username == userName);
                model = new UserVM(dto);
            }
            return View("UserProfile", model);
        }

        // POST: /account/user-profile
        [HttpPost]
        [ActionName("user-profile")]
        [Authorize]
        public ActionResult UserProfile(UserVM model)
        {
            bool userNameIsChanged = false;

            if (!ModelState.IsValid)
            {
                return View("UserProfile", model);
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (!model.Password.Equals(model.ConfirmPassword))
                {
                    ModelState.AddModelError("", "Passwords don't match.");
                    return View("UserProfile", model);
                }
            }

            using (Db db = new Db())
            {
                string userName = User.Identity.Name;

                if (userName != model.Username)
                {
                    userName = model.Username;
                    userNameIsChanged = true;
                }

                if (db.Users.Where(x => x.Id != model.Id).Any(x => x.Username == userName))
                {
                    ModelState.AddModelError("", $"Username {model.Username} already exists.");
                    model.Username = "";
                    return View("UserProfile", model);
                }

                UserDTO dto = db.Users.Find(model.Id);

                dto.FirstName = model.FirstName;
                dto.LastName = model.LastName;
                dto.EmailAdress = model.EmailAdress;
                dto.Username = model.Username;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    dto.Password = model.Password;
                }
                db.SaveChanges();
            }

            TempData["SM"] = "You have edited your profile!";

            if (!userNameIsChanged)
            {
                return View("UserProfile", model);
            }
            else
            {
                return RedirectToAction("Logout");
            }
        }

        // GET: /account/Orders
        [Authorize(Roles = "User")]
        public ActionResult Orders()
        {
            List<OrderForUserVM> orderForUser = new List<OrderForUserVM>();

            using (Db db = new Db())
            {
                // Get the ID of user
                UserDTO user = db.Users.FirstOrDefault(x => x.Username == User.Identity.Name);
                int userId = user.Id;

                List<OrderVM> orders = db.Orders.Where(x => x.UserId == userId).ToArray()
                    .Select(x => new OrderVM(x)).ToList();

                // Sorting through the list of products for OrderVM
                foreach (var order in orders)
                {
                    Dictionary<string, int> productsAndQty = new Dictionary<string, int>();

                    decimal total = 0m;
                    List<OrderDetailsDTO> orderDetailsDto = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    UserDTO userName = db.Users.FirstOrDefault(x => x.Id == order.UserId);
                    string username = user.Username;

                    // Sorting through the list OrderDetailsDTO
                    foreach (var orderDetails in orderDetailsDto)
                    {
                        ProductDTO product = db.Products.FirstOrDefault(x => x.Id == orderDetails.ProductId);

                        decimal price = product.Price;
                        string productName = product.Name;

                        productsAndQty.Add(productName, orderDetails.Quantity);
                        total += orderDetails.Quantity * price;
                    }
                    orderForUser.Add(new OrderForUserVM()
                    {
                        OrderNumber = order.OrderId,
                        UserName = username,
                        Total = total,
                        ProductsAndQty = productsAndQty,
                        CreatedAt = order.CreatedAt
                    });
                }
            }
            return View(orderForUser);
        }
    }
}