using EMS.EMS.Domain.Entities;

namespace EMS.EMS.Domain.Interfaces
{
    public interface ILeaveRepository
    {
        Task ApplyLeaveAsync(Leave leave);
        Task<Employee?> GetEmployeeByUserIdAsync(int? userId);
        Task<Leave?> GetLeaveByIdAsync(int leaveId);
        Task<bool> UpdateLeaveAsync(Leave leave);
        Task<Employee?> GetEmployeeByIdAsync(int employeeId);
        Task<int> GetLeaveBalanceAsync(int employeeId);
        Task<List<Leave>> GetPendingLeaveRequestsAsync();


    }
}
