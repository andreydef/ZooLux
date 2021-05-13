using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebStore_Contrast.Models.Data
{
    [Table("tblFeatures")]
    public class FeaturesDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string Id_property { get; set; }
        public string Id_value { get; set; }

        // Assign a foreign key
        [ForeignKey("CategoryId")]
        public virtual CategoryDTO Category { get; set; }
    }
}