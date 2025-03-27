using EMS.EMS.Application.DTOs.TimeSheetDTO;

namespace EMS.EMS.Application.DTOs.EmployeeDTO
{
    public class EmployeeDetailsWithTimesheetsDto
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public List<TimesheetDto> Timesheets { get; set; } = new();
    }
}