using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebStore_Contrast.Models.Data
{
    [Table("tblBrands")]
    public class BrandsDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Short_desc { get; set; }
        public string Meta_title { get; set; }
        public string Meta_keywords { get; set; }
        public string Meta_description { get; set; }
        public string Body { get; set; }
        public string ImageName { get; set; }
    }
}