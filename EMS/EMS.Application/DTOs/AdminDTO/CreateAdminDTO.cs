using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Application.DTOs.AdminDTO
{
    public class CreateAdminDTO
    {
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
        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number. Include country code if needed.")]
        public required string Phone { get; set; }
    }
}