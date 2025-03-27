using EMS.EMS.Application.DTOs.TimeSheetDTO;

namespace EMS.EMS.Application.DTOs.DashboardDTO
{
    public class EmployeeDashboardDto
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public double TotalHoursLogged { get; set; }
        public int LeaveBalance { get; set; }
        public List<TimesheetDashDto> LatestTimesheets { get; set; }
    }
}