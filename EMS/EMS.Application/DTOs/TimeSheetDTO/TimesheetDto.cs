namespace EMS.EMS.Application.DTOs.TimeSheetDTO
{
    public class TimesheetDto
    {
        public int TimesheetId { get; set; }
        public DateOnly Date { get; set; }
        public double HoursWorked { get; set; }
    }
}