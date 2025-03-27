

namespace EMS.EMS.Application.DTOs.EmployeeDTO
{
    public class EmployeeResponseDTO
    {
        public int UserId { get; set; }
        public int EmployeeId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DepartmentName { get; set; }
        public string? TechStack { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public int LeaveBalance { get; set; }
    }
}
