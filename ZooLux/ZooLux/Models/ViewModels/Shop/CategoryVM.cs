using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using ZooLux.Models.Data;

namespace ZooLux.Models.ViewModels.Shop
{
    public class CategoryVM
    {
        public CategoryVM() { }

        public CategoryVM(CategoryDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Short_desc = row.Short_desc;
            Meta_title = row.Meta_title;
            Meta_keywords = row.Meta_keywords;
            Meta_description = row.Meta_description;
            Body = row.Body;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Short_desc { get; set; }
        public string Meta_title { get; set; }
        public string Meta_keywords { get; set; }
        public string Meta_description { get; set; }
        [AllowHtml]
        public string Body { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}