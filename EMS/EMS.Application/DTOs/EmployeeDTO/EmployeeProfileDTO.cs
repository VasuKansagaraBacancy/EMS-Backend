namespace EMS.EMS.Application.DTOs.EmployeeDTO
{
    public class EmployeeProfileDTO
    {
        public int EmployeeId { get; set; }
        public string DepartmentName { get; set; }
        public string TechStack { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; }
        public int Leavebalance { get; set; }
    }
}