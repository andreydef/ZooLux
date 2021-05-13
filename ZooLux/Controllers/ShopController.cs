using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Shop;

namespace WebStore_Contrast.Controllers
{
    public class ShopController : Controller
    {
        // GET: Shop
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Pages");
        }

        // GET: Shop/CategoryMenuPartial
        public ActionResult CategoryMenuPartial(CategoryVM model)
        {
            // Assign the model with type List<> CategoryVM
            List<CategoryVM> categoryVMList;

            // Initialize the model to data
            using (Db db = new Db())
            {
                categoryVMList = db.Categories.ToArray()
                    .Select(x => new CategoryVM(x)).ToList();

                if (db.Categories.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Return PartialView() with model 
            return PartialView("_CategoryMenuPartial", categoryVMList);
        }

        // GET: Shop/BrandMenuPartial
        public ActionResult BrandMenuPartial(BrandsVM model)
        {
            // Assign the model with type List<> BrandsVM
            List<BrandsVM> brandVMList;

            // Initialize the model to data
            using (Db db = new Db())
            {
                brandVMList = db.Brands.ToArray()
                    .Select(x => new BrandsVM(x)).ToList();

                if (db.Brands.Any(x => x.Name == model.Name))
                {
                    model.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Return PartialView() with model 
            return PartialView("_BrandMenuPartial", brandVMList);
        }

        // GET: Shop/ProductMenuPartial
        public ActionResult ProductMenuPartial(int? page, int? catId)
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
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3); // the number of goods in page
            ViewBag.onePageOfProducts = onePageOfProducts;

            // Return PartialView() with model 
            return PartialView("_ProductMenuPartial", listOfProductVM);
        }

        // GET: Shop/SmallProductMenuPartial
        public ActionResult SmallProductMenuPartial(int? page, int? catId)
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
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3); // the number of goods in page
            ViewBag.onePageOfProducts = onePageOfProducts;

            // Return PartialView() with model 
            return PartialView("_SmallProductMenuPartial", listOfProductVM);
        }

        // GET: Shop/FeaturesMenuPartial
        public ActionResult FeaturesMenuPartial(FeaturesVM model)
        {
            // Assign the model with type List<> FeaturesVM
            List<FeaturesVM> featuresVMList;

            // Initialize the model to data
            using (Db db = new Db())
            {
                featuresVMList = db.Features.ToArray()
                    .Select(x => new FeaturesVM(x)).ToList();

                if (db.Features.Any(x => x.Name == model.Name))
                {
                    model.Features = new SelectList(db.Features.ToList(), "Id", "Name");
                    return View(model);
                }
            }

            // Return PartialView() with model 
            return PartialView("_FeaturesMenuPartial", featuresVMList);
        }

        // GET: Shop/product-details/name
        [ActionName("product-details")]
        public ActionResult ProductDetails(string name)
        {
            // Assign the models DTO and VM
            ProductDTO dto;
            ProductVM model;

            // Assign and initialize ID of product
            int id = 0;

            using (Db db = new Db())
            {
                // Check, that the product is available 
                if (!db.Products.Any(x => x.Slug.Equals(name)))
                {
                    return RedirectToAction("Index", "Shop");
                }

                // Initialize the model DTO to data
                dto = db.Products.Where(x => x.Slug == name).FirstOrDefault();

                // Get ID
                id = dto.Id;

                // Initialize the model VM to data 
                model = new ProductVM(dto);
            }

            // Get the images from gallery
            model.GalleryImages = Directory
                .EnumerateFiles(Server.MapPath("~/Images/Uploads/Products/" + id + "/Gallery/Thumbs"))
                .Select(fn => Path.GetFileName(fn));

            // Return the model in View()
            return View("ProductDetails", model);
        }

        // Add GET method the list of goods
        // GET: Shop/Products
        [HttpGet]
        public ActionResult Products(int? page, int? catId)
        {
            // Assign model ProductVM with type List
            List<ProductVM> listOfProductVM;

            // Assign model ProductVM with type List
            List<BrandsVM> listOfBrandVM;

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

                // Initialize List and fill in data
                listOfBrandVM = db.Brands.ToArray()
                    .Where(x => catId == null || catId == 0 || x.Id == catId)
                    .Select(x => new BrandsVM(x))
                    .ToList();

                // Fill in the categories with data
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Fill in the brands with data
                ViewBag.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");

                // Set the selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            // Set a page navigation
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3); // the number of goods in page
            ViewBag.onePageOfProducts = onePageOfProducts;

            // Set a page navigation
            var onePageOfBrands = listOfBrandVM.ToPagedList(pageNumber, 6); // the number of brands in page
            ViewBag.onePageOfBrands = onePageOfBrands;

            // Return View() with data
            return View(listOfProductVM);
        }

        // Add GET method the list of brands
        // GET: Shop/Products
        [HttpGet]
        public ActionResult Brands(int? page, int? brandId)
        {
            // Assign model BrandsVM with type List
            List<BrandsVM> listOfBrandVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfBrandVM = db.Brands.ToArray()
                    .Where(x => brandId == null || brandId == 0 || x.Id == brandId)
                    .Select(x => new BrandsVM(x))
                    .ToList();

                // Fill in the brands with data
                ViewBag.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");

                // Set the selected brand
                ViewBag.SelectedBrand = brandId.ToString();
            }

            // Set a page navigation
            var onePageOfBrands = listOfBrandVM.ToPagedList(pageNumber, 6); // the number of brands in page
            ViewBag.onePageOfBrands = onePageOfBrands;

            // Return View() with data
            return View(listOfBrandVM);
        }

        // Add GET method the list of features
        // GET: Shop/Features
        [HttpGet]
        public ActionResult Features(int? page, int? featureId)
        {
            // Assign model FeaturesVM with type List
            List<FeaturesVM> listOfFeatureVM;

            // Assign model ProductVM with type List
            List<BrandsVM> listOfBrandVM;

            // Set the number of page
            var pageNumber = page ?? 1; /* if the result returns null it will automatically be set to 1,
                                               if it returns a value instead of 1 it will be this value */

            using (Db db = new Db())
            {
                // Initialize List and fill in data
                listOfFeatureVM = db.Features.ToArray()
                    .Where(x => featureId == null || featureId == 0 || x.Id == featureId)
                    .Select(x => new FeaturesVM(x))
                    .ToList();

                // Initialize List and fill in data
                listOfBrandVM = db.Brands.ToArray()
                    .Where(x => featureId == null || featureId == 0 || x.Id == featureId)
                    .Select(x => new BrandsVM(x))
                    .ToList();

                // Fill in the categories with data
                ViewBag.Features = new SelectList(db.Features.ToList(), "Id", "Name");

                // Fill in the brands with data
                ViewBag.Brands = new SelectList(db.Brands.ToList(), "Id", "Name");

                // Set the selected features
                ViewBag.SelectedFeatures = featureId.ToString();
            }

            // Set a page navigation
            var onePageOfFeatures = listOfFeatureVM.ToPagedList(pageNumber, 3); // the number of goods in page
            ViewBag.onePageOfFeatures = onePageOfFeatures;

            // Set a page navigation
            var onePageOfBrands = listOfBrandVM.ToPagedList(pageNumber, 6); // the number of brands in page
            ViewBag.onePageOfBrands = onePageOfBrands;

            // Return View() with data
            return View(listOfFeatureVM);
        }
    }
}