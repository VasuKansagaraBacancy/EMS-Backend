namespace EMS.EMS.Application.DTOs.DashboardDTO
{
    public class AdminDashboardDto
    {
        public int TotalEmployees { get; set; }
        public List<LeaveRequestDashDto> PendingLeaveRequests { get; set; }

    }

}
