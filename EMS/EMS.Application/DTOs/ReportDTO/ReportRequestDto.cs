namespace EMS.EMS.Application.DTOs.ReportDTO
{
    public class ReportRequestDto
    {
        public int? EmployeeId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string GroupBy { get; set; } = "weekly";
    }
}