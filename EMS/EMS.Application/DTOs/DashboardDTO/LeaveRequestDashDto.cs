namespace EMS.EMS.Application.DTOs.DashboardDTO
{
    public class LeaveRequestDashDto
    {
        public int EmployeeId { get; set; }
        public string LeaveType { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
