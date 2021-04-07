using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ZooLux.Models.Data;
using ZooLux.Models.ViewModels.Shop;

namespace ZooLux.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        #region Categories

        // GET: Admin/Shop
        public ActionResult Categories()
        {
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                // Initialize model to data
                categoryVMList = db.Categories
                    .ToArray()
                    .Select(x => new CategoryVM(x))
                    .ToList();
            }
            return View(categoryVMList);
        }

        // POST : Admin/Shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            string id;

            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == catName))
                    return "titletaken";

                CategoryDTO dto = new CategoryDTO();

                dto.Name = catName;
                dto.Short_desc = catName;
                db.Categories.Add(dto);
                db.SaveChanges();
                id = dto.Id.ToString();
            }
            return id;
        }

        // GET: Admin/Shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                CategoryDTO dto = db.Categories.Find(id);
                db.Categories.Remove(dto);
                db.SaveChanges();
            }
            TempData["SM"] = "You have deleted a category!";
            return RedirectToAction("Categories");
        }

        // POST: Admin/Shop/RenameCategory/id
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {
            using (Db db = new Db())
            {
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";

                CategoryDTO dto = db.Categories.Find(id);
                dto.Name = newCatName;
                dto.Short_desc = newCatName;
                db.SaveChanges();
            }
            return "ok";
        }

        #endregion
    }
}