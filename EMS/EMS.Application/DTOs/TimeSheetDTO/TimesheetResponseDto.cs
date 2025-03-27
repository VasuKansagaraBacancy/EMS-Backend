namespace EMS.EMS.Application.DTOs.TimeSheetDTO
{
    public class TimesheetResponseDto
    {
        public int TimesheetId { get; set; }
        public int EmployeeId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal TotalHours { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}