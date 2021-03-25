using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooLux.Models.Data
{
    [Table("Products")]
    public class ProductDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public string ImageName { get; set; }
        public int Brands_id { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CategoryDTO Category { get; set; }

        [ForeignKey("Brands_id")]
        public virtual BrandsDTO Brand { get; set; }
    }
}