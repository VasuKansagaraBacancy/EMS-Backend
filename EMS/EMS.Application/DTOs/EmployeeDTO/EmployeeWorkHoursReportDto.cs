namespace EMS.EMS.Application.DTOs.EmployeeDTO
{
    public class EmployeeWorkHoursReportDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public double TotalHours { get; set; }
    }
}