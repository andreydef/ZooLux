using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebStore_Contrast.Areas.Admin.Models.ViewModels.Shop;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Shop;

namespace WebStore_Contrast.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        #region Products

        // Add GET method the list of goods
        // GET: Admin/Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            // Assign model ProductVM with type List
            List<ProductVM> listOfProductVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfProductVM = db.Products.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new ProductVM(x))
                    .ToList();

                // Fill in the categories with data
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Set the selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            // Set a page navigation
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 4); // the number of goods in page
            ViewBag.onePageOfProducts = onePageOfProducts;

            // Return View() with data
            return View(listOfProductVM);
        }

        // Add GET method to Adding goods
        // GET: Admin/Shop/AddProduct
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Assign the model of data
            ProductVM model = new ProductVM();

            // Add the list of categories from database to model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            // Return model in view()
            return View(model);
        }

        // Add POST method to Adding goods
        // POST: Admin/Shop/AddProduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            // Check model in validation
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Check the product name for unicity
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The product name is taken!");
                    return View(model);
                }
            }
            // Assign variable ProductID
            int id;

            // Initialize and save model on base ProductDTO
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();
                id = product.Id;
            }

            // Add message in TempData
            TempData["SM"] = "You have added a product!";

            #region Upload Image

            // Create the necessary links of directories
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            // Check availability of directories (if not, create)
            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Check that the file has been downloaded 
            if (file != null && file.ContentLength > 0)
            {
                // Get the file extension
                string ext = file.ContentType.ToLower();

                // Check file extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png" &&
                    ext != "image/xbm" &&
                    ext != "image/tif" &&
                    ext != "image/pjp" &&
                    ext != "image/jfif" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/ico" &&
                    ext != "image/tiff" &&
                    ext != "image/svg" &&
                    ext != "image/bmp" &&
                    ext != "image/svgz" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/webp")   // A few (рідкісне) image extension but sometimes used
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension!");
                        return View(model);
                    }
                }

                // Assign variable with name of image
                string imageName = file.FileName;

                // Save the name of image in model DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Assign paths to the original and reduced image
                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                // Save original image
                file.SaveAs(path);

                // Create and save reduced copy of image
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }
            #endregion

            // Redirect user 
            return RedirectToAction("AddProduct");
        }

        // Add GET method to Edit Products
        // GET: Admin/Shop/EditProduct/id
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            // Assign model ProductVM
            ProductVM model;

            using (Db db = new Db())
            {
                // Get product (goods)
                ProductDTO dto = db.Products.Find(id);

                // Check, that the product is available 
                if (dto == null)
                {
                    return Content("That product does not exist!");
                }

                // Initialize model to data
                model = new ProductVM(dto);

                // Create the list of categories
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Get all images from gallery
                model.GalleryImages = Directory
                    .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                    .Select(fn => Path.GetFileName(fn));
            }

            // Return model in View()
            return View(model);
        }

        // Add POST method to Edit Products
        // POST: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditProduct(ProductVM model, HttpPostedFileBase file)
        {
            // Get ID of product 
            int id = model.Id;

            // Fill in the List with categories and images
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check the name of product in unicity
            using (Db db = new Db())
            {
                if (db.Products.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That product name is taken!");
                    return View(model);
                }
            }

            // Update product 
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);

                dto.Name = model.Name;
                dto.Slug = model.Name;
                dto.Description = model.Description;
                dto.Price = model.Price;
                dto.CategoryId = model.CategoryId;
                dto.ImageName = model.ImageName;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            // Set the message in TempData
            TempData["SM"] = "You have edited the product!";

            // Realize the logic of image processing

            #region Image Upload

            // Check the file downloading
            if (file != null && file.ContentLength > 0)
            {
                // Get the image extension
                string ext = file.ContentType.ToLower();

                // Check the extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png" &&
                    ext != "image/xbm" &&
                    ext != "image/tif" &&
                    ext != "image/pjp" &&
                    ext != "image/jfif" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/ico" &&
                    ext != "image/tiff" &&
                    ext != "image/svg" &&
                    ext != "image/bmp" &&
                    ext != "image/svgz" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/webp")   // A few (рідкісне) image extension but sometimes used
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension!");
                        return View(model);
                    }
                }

                // Set path for download
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");

                // Delete the existent files and directories 
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                // Save the image
                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Save the original and preview version 
                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

                // Save original image
                file.SaveAs(path);

                // Create and save reduced copy of image
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }
            #endregion

            // Redirect user
            return RedirectToAction("EditProduct");
        }

        // Add GET method to Delete Products
        // GET: Admin/Shop/DeleteProduct/id
        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            // Delete product from database 
            using (Db db = new Db())
            {
                ProductDTO dto = db.Products.Find(id);
                db.Products.Remove(dto);
                db.SaveChanges();
            }

            // Delete the directories of product (images)
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));
            var pathString = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());

            if (Directory.Exists(pathString))
                Directory.Delete(pathString, true);

            // Redirect user
            return RedirectToAction("Products");
        }

        // Add POST method to Add images to Gallery
        // POST: Admin/Shop/SaveGalleryImages/id
        [HttpPost]
        public void SaveGalleryImages(int id)
        {
            // Sort all getting files
            foreach (string fileName in Request.Files)
            {
                // Initialize the files
                HttpPostedFileBase file = Request.Files[fileName];

                // Check on null
                if (file != null && file.ContentLength > 0)
                {
                    // Assign all paths to directories
                    var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                    string pathString1 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
                    string pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

                    // Assign paths of images
                    var path = string.Format($"{pathString1}\\{file.FileName}");
                    var path2 = string.Format($"{pathString2}\\{file.FileName}");

                    // Save original and reduced copies of images
                    file.SaveAs(path);

                    WebImage img = new WebImage(file.InputStream);
                    img.Resize(200, 200).Crop(1, 1);
                    img.Save(path2);
                }
            }
        }

        // Add POST method to Delete images from Gallery
        // POST: Admin/Shop/DeleteImage/id/imageName
        [HttpPost]
        public void DeleteImage(int id, string imageName)
        {
            string fullPath1 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/" + imageName);
            string fullPath2 = Request.MapPath("~/Images/Uploads/Products/" + id.ToString() + "/Gallery/Thumbs" + imageName);

            // Check that the images is available 
            if (System.IO.File.Exists(fullPath1))
                System.IO.File.Delete(fullPath1);

            if (System.IO.File.Exists(fullPath2))
                System.IO.File.Delete(fullPath2);
        }

        #endregion

        #region Categories

        // Add GET method the list of categories
        // GET: Admin/Shop/Categories
        [HttpGet]
        public ActionResult Categories(int? page, int? catId)
        {
            // Assign model CategoryVM with type List
            List<CategoryVM> listOfCatVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfCatVM = db.Categories.ToArray()
                    .Where(x => catId == null || catId == 0 || x.Id == catId)
                    .Select(x => new CategoryVM(x))
                    .ToList();

                // Fill in the categories with data
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Set the selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            // Set a page navigation
            var onePageOfCategories = listOfCatVM.ToPagedList(pageNumber, 3); // the number of categories in page
            ViewBag.onePageOfCategories = onePageOfCategories;

            // Return View() with data
            return View(listOfCatVM);
        }

        // Add GET method to Adding categories
        // GET: Admin/Shop/AddCategory
        [HttpGet]
        public ActionResult AddCategory()
        {
            // Assign the model of data
            CategoryVM model = new CategoryVM();

            // Add the list of categories from database to model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            // Return model in view()
            return View(model);
        }

        // Add POST method to Adding categories
        // POST: Admin/Shop/AddCategory
        [HttpPost]
        public ActionResult AddCategory(CategoryVM model, HttpPostedFileBase file)
        {
            // Check model in validation
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Check the category name for unicity
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The category name is taken!");
                    return View(model);
                }
            }

            // Assign variable CategoryID
            int id;

            // Initialize and save model on base CategoryDTO
            using (Db db = new Db())
            {
                CategoryDTO category = new CategoryDTO();

                category.Name = model.Name;
                category.Short_desc = model.Short_desc;
                category.Meta_title = model.Meta_title;
                category.Meta_keywords = model.Meta_keywords;
                category.Meta_description = model.Meta_description;
                category.Body = model.Body;

                db.Categories.Add(category);
                db.SaveChanges();

                id = category.Id;
            }

            // Add message in TempData
            TempData["SM"] = "You have added a category!";

            // Redirect user 
            return RedirectToAction("AddCategory");
        }

        // Add GET method to Edit Categories
        // GET: Admin/Shop/EditCategory/id
        [HttpGet]
        public ActionResult EditCategory(int id)
        {
            // Assign model CategoryVM
            CategoryVM model;

            using (Db db = new Db())
            {
                // Get category
                CategoryDTO dto = db.Categories.Find(id);

                // Check, that the category is available 
                if (dto == null)
                {
                    return Content("That category does not exist!");
                }

                // Initialize model to data
                model = new CategoryVM(dto);

                // Create the list of categories
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // Return model in View()
            return View(model);
        }

        // Add POST method to Edit Categories
        // POST: Admin/Shop/EditCategory
        [HttpPost]
        public ActionResult EditCategory(CategoryVM model, HttpPostedFileBase file)
        {
            // Get ID of category
            int id = model.Id;

            // Fill in the List with categories
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check the name of category in unicity
            using (Db db = new Db())
            {
                if (db.Categories.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That category name is taken!");
                    return View(model);
                }
            }

            // Update category
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);

                dto.Name = model.Name;
                dto.Short_desc = model.Short_desc;
                dto.Meta_title = model.Meta_title;
                dto.Meta_keywords = model.Meta_keywords;
                dto.Meta_description = model.Meta_description;
                dto.Body = model.Body;

                db.SaveChanges();
            }

            // Set the message in TempData
            TempData["SM"] = "You have edited the category!";

            // Redirect user
            return RedirectToAction("EditCategory");
        }

        // Add GET method to Delete Category
        // GET: Admin/Shop/DeleteCategory/id
        [HttpGet]
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                // Get the model of category
                CategoryDTO dto = db.Categories.Find(id);

                // Delete category
                db.Categories.Remove(dto);

                // Save changes in database
                db.SaveChanges();
            }

            // Add message about successful delete
            TempData["SM"] = "You have deleted a category!";

            // Return user to the page Categories
            return RedirectToAction("Categories");
        }

        #endregion

        #region Brands

        // Add GET method the list of brands
        // GET: Admin/Shop/Brands
        [HttpGet]
        public ActionResult Brands(int? page)
        {
            // Assign model BrandVM with type List
            List<BrandsVM> listOfBrandVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfBrandVM = db.Brands.ToArray()
                    .Select(x => new BrandsVM(x))
                    .ToList();
            }

            // Set a page navigation
            var onePageOfBrands = listOfBrandVM.ToPagedList(pageNumber, 5); // 5 - the number of brands in page
            ViewBag.onePageOfBrands = onePageOfBrands;

            // Return View() with data
            return View(listOfBrandVM);
        }

        // Add GET method to Adding brands
        // GET: Admin/Shop/AddBrand
        [HttpGet]
        public ActionResult AddBrand()
        {
            // Assign the model of data
            BrandsVM model = new BrandsVM();

            // Add the list of brands from database to model
            using (Db db = new Db())
            {
                model.Brands = new SelectList(db.Brands.ToList(), "id", "Name");
            }

            // Return model in view()
            return View(model);
        }

        // Add POST method to Adding brands
        // POST: Admin/Shop/AddBrand
        [HttpPost]
        public ActionResult AddBrand(BrandsVM model, HttpPostedFileBase file)
        {
            // Check model in validation
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Check the brand name for unicity
            using (Db db = new Db())
            {
                if (db.Brands.Any(x => x.Name == model.Name))
                {
                    model.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The brand name is taken!");
                    return View(model);
                }
            }

            // Assign variable BrandID
            int id;

            // Initialize and save model on base BrandDTO
            using (Db db = new Db())
            {
                BrandsDTO brand = new BrandsDTO();

                brand.Name = model.Name;
                brand.Short_desc = model.Short_desc;
                brand.Meta_title = model.Meta_title;
                brand.Meta_keywords = model.Meta_keywords;
                brand.Meta_description = model.Meta_description;
                brand.Body = model.Body;
                brand.ImageName = model.ImageName;

                db.Brands.Add(brand);
                db.SaveChanges();

                id = brand.Id;
            }

            #region Upload Image

            // Create the necessary links of directories
            var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Brands");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Brands\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Brands\\" + id.ToString() + "\\Thumbs");

            // Check availability of directories (if not, create)
            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            // Check that the file has been downloaded 
            if (file != null && file.ContentLength > 0)
            {
                // Get the file extension
                string ext = file.ContentType.ToLower();

                // Check file extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png" &&
                    ext != "image/xbm" &&
                    ext != "image/tif" &&
                    ext != "image/pjp" &&
                    ext != "image/jfif" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/ico" &&
                    ext != "image/tiff" &&
                    ext != "image/svg" &&
                    ext != "image/bmp" &&
                    ext != "image/svgz" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/webp")   // A few (рідкісне) image extension but sometimes used
                {
                    using (Db db = new Db())
                    {
                        model.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension!");
                        return View(model);
                    }
                }

                // Assign variable with name of image
                string imageName = file.FileName;

                // Save the name of image in model DTO
                using (Db db = new Db())
                {
                    BrandsDTO dto = db.Brands.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Assign paths to the original and reduced image
                var path = string.Format($"{pathString2}\\{imageName}");
                var path2 = string.Format($"{pathString3}\\{imageName}");

                // Save original image
                file.SaveAs(path);

                // Create and save reduced copy of image
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }
            #endregion

            // Add message in TempData
            TempData["SM"] = "You have added a brand!";

            // Redirect user 
            return RedirectToAction("AddBrand");
        }

        // Add GET method to Edit Brands
        // GET: Admin/Shop/EditBrand/id
        [HttpGet]
        public ActionResult EditBrand(int id)
        {
            // Assign model BrandVM
            BrandsVM model;

            using (Db db = new Db())
            {
                // Get brands
                BrandsDTO dto = db.Brands.Find(id);

                // Check, that the brand is available 
                if (dto == null)
                {
                    return Content("That brand does not exist!");
                }

                // Initialize model to data
                model = new BrandsVM(dto);
            }

            // Return model in View()
            return View(model);
        }

        // Add POST method to Edit Brands
        // POST: Admin/Shop/EditBrand
        [HttpPost]
        public ActionResult EditBrand(BrandsVM model, HttpPostedFileBase file)
        {
            // Get ID of product 
            int id = model.Id;

            // Fill in the List with brands
            using (Db db = new Db())
            {
                model.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");
            }

            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check the name of product in unicity
            using (Db db = new Db())
            {
                if (db.Brands.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That brand name is taken!");
                    return View(model);
                }
            }

            // Update product 
            using (Db db = new Db())
            {
                BrandsDTO dto = db.Brands.Find(id);

                dto.Name = model.Name;
                dto.Short_desc = model.Short_desc;
                dto.Meta_title = model.Meta_title;
                dto.Meta_keywords = model.Meta_keywords;
                dto.Meta_description = model.Meta_description;
                dto.Body = model.Body;
                dto.ImageName = model.ImageName;

                db.SaveChanges();
            }

            // Set the message in TempData
            TempData["SM"] = "You have edited the brand!";

            // Realize the logic of image processing

            #region Image Upload

            // Check the file downloading
            if (file != null && file.ContentLength > 0)
            {
                // Get the image extension
                string ext = file.ContentType.ToLower();

                // Check the extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png" &&
                    ext != "image/xbm" &&
                    ext != "image/tif" &&
                    ext != "image/pjp" &&
                    ext != "image/jfif" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/ico" &&
                    ext != "image/tiff" &&
                    ext != "image/svg" &&
                    ext != "image/bmp" &&
                    ext != "image/svgz" && // A few (рідкісне) image extension but sometimes used
                    ext != "image/webp")   // A few (рідкісне) image extension but sometimes used
                {
                    using (Db db = new Db())
                    {
                        ModelState.AddModelError("", "The image was not uploaded - wrong image extension!");
                        return View(model);
                    }
                }

                // Set path for download
                var originalDirectory = new DirectoryInfo(string.Format($"{Server.MapPath(@"\")}Images\\Uploads"));

                var pathString1 = Path.Combine(originalDirectory.ToString(), "Brands\\" + id.ToString());
                var pathString2 = Path.Combine(originalDirectory.ToString(), "Brands\\" + id.ToString() + "\\Thumbs");

                // Delete the existent files and directories 
                DirectoryInfo di1 = new DirectoryInfo(pathString1);
                DirectoryInfo di2 = new DirectoryInfo(pathString2);

                foreach (var file2 in di1.GetFiles())
                {
                    file2.Delete();
                }

                foreach (var file3 in di2.GetFiles())
                {
                    file3.Delete();
                }

                // Save the image
                string imageName = file.FileName;

                using (Db db = new Db())
                {
                    BrandsDTO dto = db.Brands.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Save the original and preview version 
                var path = string.Format($"{pathString1}\\{imageName}");
                var path2 = string.Format($"{pathString2}\\{imageName}");

                // Save original image
                file.SaveAs(path);

                // Create and save reduced copy of image
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200).Crop(1, 1);
                img.Save(path2);
            }
            #endregion

            // Redirect user
            return RedirectToAction("EditBrand");
        }

        // Add GET method to Delete Brand
        // GET: Admin/Shop/DeleteBrand/id
        [HttpGet]
        public ActionResult DeleteBrand(int id)
        {
            using (Db db = new Db())
            {
                // Get the model of brand
                BrandsDTO dto = db.Brands.Find(id);

                // Delete brand
                db.Brands.Remove(dto);

                // Save changes in database
                db.SaveChanges();
            }

            // Add message about successful delete
            TempData["SM"] = "You have deleted a brand!";

            // Return user to the page Brands
            return RedirectToAction("Brands");
        }

        #endregion

        #region Features

        // Add GET method the list of features
        // GET: Admin/Shop/Features
        [HttpGet]
        public ActionResult Features(int? page, int? catId)
        {
            // Assign model FeaturesVM with type List
            List<FeaturesVM> listOfFeaturesVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfFeaturesVM = db.Features.ToArray()
                    .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                    .Select(x => new FeaturesVM(x))
                    .ToList();

                // Fill in the categories with data
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Set the selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            // Set a page navigation
            var onePageOfFeatures = listOfFeaturesVM.ToPagedList(pageNumber, 5); // 5 - the number of features in page
            ViewBag.onePageOfFeatures = onePageOfFeatures;

            // Return View() with data
            return View(listOfFeaturesVM);
        }

        // Add GET method to Adding features
        // GET: Admin/Shop/AddFeature
        [HttpGet]
        public ActionResult AddFeature()
        {
            // Assign the model of data
            FeaturesVM model = new FeaturesVM();

            // Add the list of categories from database to model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "id", "Name");
            }

            // Return model in view()
            return View(model);
        }

        // Add POST method to Adding features
        // POST: Admin/Shop/AddFeature
        [HttpPost]
        public ActionResult AddFeature(FeaturesVM model, HttpPostedFileBase file)
        {
            // Check model in validation
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Check the feature name for unicity
            using (Db db = new Db())
            {
                if (db.Features.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError("", "The feature name is taken!");
                    return View(model);
                }
            }

            // Assign variable FeatureID
            int id;

            // Initialize and save model on base FeatureDTO
            using (Db db = new Db())
            {
                FeaturesDTO feature = new FeaturesDTO();

                feature.Name = model.Name;
                feature.Url = model.Url;
                feature.Description = model.Description;
                feature.CategoryName = model.CategoryName;
                feature.CategoryId = model.CategoryId;
                feature.Id_property = model.Id_property;
                feature.Id_value = model.Id_value;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                feature.CategoryName = catDTO.Name;

                db.Features.Add(feature);
                db.SaveChanges();

                id = feature.Id;
            }

            // Add message in TempData
            TempData["SM"] = "You have added a feature!";

            // Redirect user 
            return RedirectToAction("AddFeature");
        }

        // Add GET method to Edit Features
        // GET: Admin/Shop/EditFeature/id
        [HttpGet]
        public ActionResult EditFeature(int id)
        {
            // Assign model FeatureVM
            FeaturesVM model;

            using (Db db = new Db())
            {
                // Get feature
                FeaturesDTO dto = db.Features.Find(id);

                // Check, that the feature is available 
                if (dto == null)
                {
                    return Content("That feature does not exist!");
                }

                // Initialize model to data
                model = new FeaturesVM(dto);

                // Create the list of categories
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // Return model in View()
            return View(model);
        }

        // Add POST method to Edit Features
        // POST: Admin/Shop/EditProduct
        [HttpPost]
        public ActionResult EditFeature(FeaturesVM model, HttpPostedFileBase file)
        {
            // Get ID of feature 
            int id = model.Id;

            // Fill in the List with categories
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
            }

            // Check the model in validity
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check the name of feature in unicity
            using (Db db = new Db())
            {
                if (db.Features.Where(x => x.Id != id).Any(x => x.Name == model.Name))
                {
                    ModelState.AddModelError("", "That feature name is taken!");
                    return View(model);
                }
            }

            // Update product 
            using (Db db = new Db())
            {
                FeaturesDTO dto = db.Features.Find(id);

                dto.Name = model.Name;
                dto.Url = model.Url;
                dto.Description = model.Description;
                dto.CategoryName = model.CategoryName;
                dto.CategoryId = model.CategoryId;
                dto.Id_property = model.Id_property;
                dto.Id_value = model.Id_value;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                dto.CategoryName = catDTO.Name;

                db.SaveChanges();
            }

            // Set the message in TempData
            TempData["SM"] = "You have edited the feature!";

            // Redirect user
            return RedirectToAction("EditFeature");
        }

        // Add GET method to Delete Feature
        // GET: Admin/Shop/DeleteFeature/id
        [HttpGet]
        public ActionResult DeleteFeature(int id)
        {
            using (Db db = new Db())
            {
                // Get the model of brand
                FeaturesDTO dto = db.Features.Find(id);

                // Delete brand
                db.Features.Remove(dto);

                // Save changes in database
                db.SaveChanges();
            }

            // Add message about successful delete
            TempData["SM"] = "You have deleted a feature!";

            // Return user to the page Brands
            return RedirectToAction("Features");
        }

        #endregion

        #region Orders

        // Add GET method to display the order with the products for admin
        // GET: Admin/Shop/Orders
        [HttpGet]
        public ActionResult Orders()
        {
            // Initialize the model OrdersForAdminVM
            List<OrdersForAdminVM> ordersForAdmin = new List<OrdersForAdminVM>();

            using (Db db = new Db())
            {
                // Initialize the model OrderVM
                List<OrderVM> orders = db.Orders.ToArray().Select(x => new OrderVM(x)).ToList();

                // Assign the variable of total amount
                decimal total = 0m;

                // Sorting through the data of model OrderVM
                foreach (var order in orders)
                {
                    // Initialize the dictionary of products
                    Dictionary<string, int> productAndQty = new Dictionary<string, int>();

                    // Initialize the list OrderDetailsDTO
                    List<OrderDetailsDTO> orderDetailsList = db.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();

                    // Get the name of user
                    UserDTO user = db.Users.FirstOrDefault(x => x.Id == order.UserId);
                    string username = user.Username;

                    // Sorting through the list of product with OrderDetailsDTO
                    foreach (var orderDetails in orderDetailsList)
                    {
                        // Get the product
                        ProductDTO product = db.Products.FirstOrDefault(x => x.Id == orderDetails.ProductId);

                        // Get the price of product
                        decimal price = product.Price;

                        // Get the name of product
                        string productName = product.Name;

                        // Add product to dictionary
                        productAndQty.Add(productName, orderDetails.Quantity);

                        // Get the total price of product
                        total += orderDetails.Quantity * price;
                    }
                    // Add the data to model OrdersForAdminVM
                    ordersForAdmin.Add(new OrdersForAdminVM()
                    {
                        OrderNumber = order.OrderId,
                        UserName = username,
                        Total = total,
                        ProductsAndQty = productAndQty,
                        CreatedAt = order.CreatedAt
                    });
                }
            }
            // Return the view with model OrdersForAdminVM
            return View(ordersForAdmin);
        }

        #endregion
    }
}