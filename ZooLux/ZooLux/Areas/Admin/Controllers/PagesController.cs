using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZooLux.Models.Data;
using ZooLux.Models.ViewModels.Pages;

namespace ZooLux.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            List<PageVM> pageList;

            using (Db db = new Db())
            {
                pageList = db.Pages.ToArray().OrderBy(x => x.Id)
                    .Select(x => new PageVM(x)).ToList();
            }
            return View(pageList);
        }

        #region AddPage 

        // GET: Admin/Shop/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            PageVM model = new PageVM();

            using (Db db = new Db())
            {
                model.Pages = new SelectList(db.Pages.ToList(), "id", "Title");
            }
            return View(model);
        }

        // POST: Admin/Shop/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Pages = new SelectList(db.Pages.ToList(), "Id", "Title");
                    return View(model);
                }
            }

            using (Db db = new Db())
            {
                if (db.Pages.Any(x => x.Title == model.Title))
                {
                    model.Pages = new SelectList(db.Categories.ToList(), "Id", "Title");
                    ModelState.AddModelError("", "The page name is taken!");
                    return View(model);
                }
            }

            int id;
            using (Db db = new Db())
            {
                PagesDTO page = new PagesDTO();

                page.Title = model.Title;
                page.Slug = model.Slug;
                page.Body = model.Body;

                db.Pages.Add(page);
                db.SaveChanges();

                id = page.Id;
            }
            TempData["SM"] = "You have added a page!";

            return RedirectToAction("Index");
        }

        #endregion

        #region EditPage

        // GET: Admin/Shop/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            PageVM model;

            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                if (dto == null)
                {
                    return Content("That page does not exist!");
                }
                model = new PageVM(dto);
                model.Pages = new SelectList(db.Pages.ToList(), "Id", "Title");
            }
            return View(model);
        }

        // POST: Admin/Shop/EditPage
        [HttpPost]
        public ActionResult EditPage(PageVM model, HttpPostedFileBase file)
        {
            int id = model.Id;

            using (Db db = new Db())
            {
                model.Pages = new SelectList(db.Pages.ToList(), "Id", "Name");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title))
                {
                    ModelState.AddModelError("", "That page name is taken!");
                    return View(model);
                }
            }

            // Update page
            using (Db db = new Db())
            {
                PagesDTO dto = db.Pages.Find(id);

                dto.Title = model.Title;
                dto.Slug = model.Slug;
                dto.Body = model.Body;

                db.SaveChanges();
            }
            TempData["SM"] = "You have edited the page!";

            return RedirectToAction("Index");
        }

        #endregion
    }
}