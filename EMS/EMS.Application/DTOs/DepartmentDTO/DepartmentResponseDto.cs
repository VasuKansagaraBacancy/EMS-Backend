namespace EMS.EMS.Application.DTOs.DepartmentDTO
{
    public class DepartmentResponseDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public List<EmployeeDepartmentDto> Employees { get; set; } = new List<EmployeeDepartmentDto>();
    }
}