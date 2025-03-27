namespace EMS.EMS.Application.DTOs.EmployeeDTO
{
    public class UserProfileDTO
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public EmployeeProfileDTO EmployeeDetails { get; set; }
    }
}
