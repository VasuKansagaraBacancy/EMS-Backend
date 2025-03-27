using EMS.EMS.Application.DTOs.DashboardDTO;

namespace EMS.EMS.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<EmployeeDashboardDto> GetEmployeeDashboardAsync(int employeeId);
        Task<AdminDashboardDto> GetAdminDashboardAsync();
        Task<List<ActiveEmployeeDto>> GetMostActiveEmployeesAsync(DateOnly startDate, DateOnly endDate);
        Task<LeaveAnalyticsDto> GetLeaveAnalyticsAsync();
    }
}