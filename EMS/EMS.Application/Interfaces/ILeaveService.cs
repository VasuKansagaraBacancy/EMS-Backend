using EMS.EMS.Application.DTOs.LeaveDTO;

namespace EMS.EMS.Application.Interfaces
{
    public interface ILeaveService
    {
        Task<string> ApplyForLeaveAsync(LeaveRequestDTO request, int employeeId);
        Task<string> ApproveRejectLeaveAsync(int leaveId, string action, int? adminUserId);
    }
}