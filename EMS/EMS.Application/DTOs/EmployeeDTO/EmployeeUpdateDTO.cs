using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Application.DTOs.EmployeeDTO
{
    public class EmployeeUpdateDTO
    {
        public string TechStack { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Address { get; set; }
        public int DepartmentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 digits.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number. Include country code if needed.")]
        public string Phone { get; set; }
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
    }

}
