using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooLux.Models.Data
{
    [Table("Roles")]
    public class RoleDTO
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}