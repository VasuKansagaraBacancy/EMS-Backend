using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Domain.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "RoleId is required.")]
        [ForeignKey("Role")]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number. Include country code if needed.")]
        public required string Phone { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int? CreatedBy { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int? UpdatedBy { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
        public Employee? Employee { get; set; }
        public  ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
    }
}