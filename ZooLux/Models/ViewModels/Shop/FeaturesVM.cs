using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebStore_Contrast.Models.Data;

namespace WebStore_Contrast.Models.ViewModels.Shop
{
    public class FeaturesVM
    {
        public FeaturesVM(){}

        public FeaturesVM(FeaturesDTO row)
        {
            Id = row.Id;
            Name = row.Name;
            Url = row.Url;
            Description = row.Description;
            CategoryName = row.CategoryName;
            CategoryId = row.CategoryId;
            Id_property = row.Id_property;
            Id_value = row.Id_value;
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Url { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public string CategoryName { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        public string Id_property { get; set; }
        public string Id_value { get; set; }

        public IEnumerable<SelectListItem> Features { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}