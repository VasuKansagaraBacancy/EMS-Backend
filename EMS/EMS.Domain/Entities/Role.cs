using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Domain.Entities
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [MaxLength(10, ErrorMessage = "Role name cannot exceed 10 characters.")]
        public required string RoleName { get; set; }
        public ICollection<User>? Users { get; set; }
    }
}