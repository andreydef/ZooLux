using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooLux.Models.Data
{
    [Table("Categories")]
    public class CategoryDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Short_desc { get; set; }
        public string Meta_title { get; set; }
        public string Meta_keywords { get; set; }
        public string Meta_description { get; set; }
        public string Body { get; set; }
    }
}