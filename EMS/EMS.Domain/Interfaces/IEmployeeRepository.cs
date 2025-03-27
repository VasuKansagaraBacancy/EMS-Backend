using EMS.EMS.Application.DTOs.DashboardDTO;
using EMS.EMS.Domain.Entities;
namespace EMS.EMS.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task AddAsync(Employee employee);
        Task<User> GetEmployeeByIdAsync(int employeeId);
        Task<bool> SaveChangesAsync();
        Task<bool> UpdateEmployeeAsync(Employee employee);
        Task<Employee?> GetEmployeeWithDetailsAsync(int employeeId);
        Task<List<Timesheet>> GetTimesheetsByEmployeeIdAsync(int employeeId);
        Task<int> GetTotalEmployeesAsync();
        Task<List<ActiveEmployeeDto>> GetMostActiveEmployeesAsync(DateOnly startDate, DateOnly endDate);
        Task<LeaveAnalyticsDto> GetLeaveAnalyticsAsync();
        Task<Employee?> GetEmployeeByUserIdAsync(int? userId);
        Task<bool> UpdateEmployeeByEmployeeAsync(Employee employee);
    }
}