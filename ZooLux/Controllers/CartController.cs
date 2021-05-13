using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Cart;

namespace WebStore_Contrast.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        // GET: MainCart
        public ActionResult MainCart()
        {
            // Assign the List<> with type CartVM
            var cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Check, that the cart is empty 
            if (cart.Count == 0 || Session["cart"] == null)
            {
                ViewBag.Message = "You cart is empty.";
                return View();
            }

            // Add sum and write to ViewBag
            decimal total = 0m;

            foreach (var item in cart)
            {
                total += item.Total;
            }

            ViewBag.GrandTotal = total;

            // Return List<> in View()
            return View(cart);
        }

        // GET: MainCart/AddToCartPartial/id
        [Authorize(Roles = "User")]
        public ActionResult AddToCartPartial(int id)
        {
            // Assign the List<>, with type CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM> ?? new List<CartVM>();

            // Assign the model CartVM
            CartVM model = new CartVM();

            using (Db db = new Db())
            {
                // Get product
                ProductDTO product = db.Products.Find(id);

                // Check, whether the item is already in the cart
                var productInCart = cart.FirstOrDefault(x => x.ProductId == id);

                // If no, add new product to cart
                if (productInCart == null)
                {
                    cart.Add(new CartVM()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = 1,
                        Price = product.Price,
                        Image = product.ImageName
                    });
                }
                else
                {
                    // If yes, add the item of product 
                    productInCart.Quantity++;
                }
            }

            // Get all quantity, price and add data to model
            int qty = 0;
            decimal price = 0m;

            foreach (var item in cart)
            {
                qty += item.Quantity;
                price += item.Quantity * item.Price;
            }

            model.Quantity = qty;
            model.Price = price;

            // Save the state of cart to session 
            Session["cart"] = cart;

            // Return the PartialView() with model
            return PartialView("_AddToCartPartial", model);
        }

        // GET: MainCart/IncrementProduct/productId
        public JsonResult IncrementProduct(int productId)
        {
            // Assign List<> CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Get the model CartVM from List<>
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Add quantity 
                model.Quantity++;

                // Save data 
                var result = new
                {
                    qty = model.Quantity,
                    price = model.Price
                };

                // Return JSON answer with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: MainCart/DecrementProduct/productId
        public ActionResult DecrementProduct(int productId)
        {
            // Assign List<> CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Get the model CartVM from List<>
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Take away quantity (decrement)
                if (model.Quantity > 1)
                {
                    model.Quantity--;
                }
                else
                {
                    model.Quantity = 0;
                    cart.Remove(model);
                }

                // Save data 
                var result = new
                {
                    qty = model.Quantity,
                    price = model.Price
                };

                // Return JSON answer with data
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: MainCart/RemoveProduct/productId
        public void RemoveProduct(int productId)
        {
            // Assign List<> CartVM
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            using (Db db = new Db())
            {
                // Get the model CartVM from List<>
                CartVM model = cart.FirstOrDefault(x => x.ProductId == productId);

                // Remode the model
                cart.Remove(model);
            }
        }

        // Method to update PayPal form on cart
        [Authorize(Roles = "User")]
        public ActionResult PaypalPartial()
        {
            // Get the list of products in cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Return partial view with list
            return PartialView(cart);
        }

        // POST: /cart/PlaceOrder
        [HttpPost]
        [Authorize(Roles = "User")]
        public void PlaceOrder()
        {
            // Get the list with products in cart
            List<CartVM> cart = Session["cart"] as List<CartVM>;

            // Get the name of user
            string userName = User.Identity.Name;

            // Update the variable for orderId
            int orderId = 0;

            using (Db db = new Db())
            {
                // Assign the model OrderDTO
                OrderDTO orderDTO = new OrderDTO();

                // Get the ID of user
                var name = db.Users.FirstOrDefault(x => x.Username == userName);
                int userId = name.Id;

                // Fill in the model OrderDTO to data and save changes
                orderDTO.UserId = userId;
                orderDTO.CreatedAt = DateTime.Now;

                db.Orders.Add(orderDTO);
                db.SaveChanges();

                // Get orderId
                orderId = orderDTO.OrderId;

                // Assign the model OrderDetailsDTO
                OrderDetailsDTO orderDetailsDTO = new OrderDetailsDTO();

                // Add in model some data
                foreach (var item in cart)
                {
                    orderDetailsDTO.OrderId = orderId;
                    orderDetailsDTO.UserId = userId;
                    orderDetailsDTO.ProductId = item.ProductId;
                    orderDetailsDTO.Quantity = item.Quantity;

                    db.OrderDetails.Add(orderDetailsDTO);
                    db.SaveChanges();
                }
            }
            // Send a letter about order to the administrator's mail
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("cd701fcc6a14ab", "4f619e9e2d0fd9"),
                EnableSsl = true
            };
            client.Send("from@example.com", "admin@example.com", "New Order", $"You have a new order. Order number: {orderId}");

            // Update the session
            Session["cart"] = null;

            Server.ScriptTimeout = 2000;
        }
    }
}