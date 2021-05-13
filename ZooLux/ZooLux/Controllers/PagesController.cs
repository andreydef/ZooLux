using System.Linq;
using System.Web.Mvc;
using ZooLux.Models.Data;
using ZooLux.Models.ViewModels.Pages;

namespace ZooLux.Controllers
{
    public class PagesController : Controller
    {
        // GET: Index/{page}
        public ActionResult Index(string page = "")
        {
            if (page == "")
            {
                page = "home";
            }

            PageVM model;
            PagesDTO dto;

            // Check, that the page is available
            using (Db db = new Db())
            {
                if (!db.Pages.Any(x => x.Slug.Equals(page)))
                {
                    return RedirectToAction("Index", new { page = "" });
                }
            }

            using (Db db = new Db())
            {
                dto = db.Pages.Where(x => x.Slug == page).FirstOrDefault();
            }

            ViewBag.PageTitle = dto.Title;
            model = new PageVM(dto);

            return View(model);
        }
    }
}