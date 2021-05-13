using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;
using WebStore_Contrast.Models.ViewModels.Pages;
using WebStore_Contrast.Models.ViewModels.Shop;

namespace WebStore_Contrast.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index()
        {
            return View();
        }

        // GET: Pages/PagesMenuPartial
        public ActionResult PagesMenuPartial()
        {
            // Initialize list PageVM
            List<PageVM> pageVMList;

            // Get all pages, except HOME
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().Where(x => x.Slug != "home")
                    .Select(x => new PageVM(x)).ToList();
            }

            // Return partial view() with list of data
            return PartialView("_PagesMenuPartial", pageVMList);
        }

        // GET: Pages/PagesRedirect
        public ActionResult PagesRedirect()
        {
            // Initialize list PageVM
            List<PageVM> pageVMList;

            // Get all pages, except HOME
            using (Db db = new Db())
            {
                pageVMList = db.Pages.ToArray().Where(x => x.Slug != "home")
                    .Select(x => new PageVM(x)).ToList();
            }

            return View("PagesRedirect", pageVMList);
        }

        // GET: Pages/Products
        public ActionResult Products(int? page)
        {
            return View("Products");
        }

        // GET: Pages/Brands
        public ActionResult Brands(int? page)
        {
            return View("Brands");
        }

        // GET: Pages/Features
        public ActionResult Features(int? page)
        {
            return View("Features");
        }
    }
}