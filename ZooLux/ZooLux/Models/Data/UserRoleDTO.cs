using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZooLux.Models.Data
{
    [Table("UserRoles")]
    public class UserRoleDTO
    {
        [Key, Column(Order = 0)]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        public int RoleId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserDTO User { get; set; }

        [ForeignKey("RoleId")]
        public virtual RoleDTO Role { get; set; }
    }
}