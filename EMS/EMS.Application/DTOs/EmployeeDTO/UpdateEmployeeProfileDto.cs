using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Application.DTOs.EmployeeDTO
{
    public class UpdateEmployeeProfileDto
    {
        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number. Include country code if needed.")]
        public required string Phone { get; set; }
        public required string TechStack { get; set; } 
        public required string Address { get; set; }
    }
}
