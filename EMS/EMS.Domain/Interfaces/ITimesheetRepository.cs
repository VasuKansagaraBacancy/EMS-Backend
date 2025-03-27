using EMS.EMS.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EMS.EMS.Domain.Interfaces
{
    public interface ITimesheetRepository
    {

        Task<IEnumerable<Timesheet>> GetTimesheetsByEmployeeIdAsync(int employeeId);
        Task<Timesheet?> GetTimesheetByIdAsync(int timesheetId);
        Task AddTimesheetAsync(Timesheet timesheet);
        Task UpdateTimesheetAsync(Timesheet timesheet);
        Task<bool> SaveChangesAsync();
        Task<Employee?> GetEmployeeByUserIdAsync(int? userId);
        Task<Timesheet?> GetTimesheetByEmployeeIdAndDateAsync(int employeeId, DateOnly date);
        Task<double> GetTotalLoggedHoursAsync(int employeeId);
        Task<List<Timesheet>> GetLatestTimesheetsAsync(int employeeId, int count);
    }
}
