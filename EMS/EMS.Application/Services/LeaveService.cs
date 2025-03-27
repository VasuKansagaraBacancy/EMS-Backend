using EMS.EMS.Application.DTOs.LeaveDTO;
using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;

namespace EMS.EMS.Application.Services
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IEmployeeRepository _empRepo;
        public LeaveService(ILeaveRepository leaveRepository, IEmployeeRepository empRepo)
        {
            _leaveRepository = leaveRepository;
            _empRepo = empRepo;
        }
        public async Task<string> ApplyForLeaveAsync(LeaveRequestDTO request, int userId)
        {
            try
            {
                if (request == null)
                    throw new ArgumentException("Leave request cannot be null.");

                if (request.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
                    throw new ArgumentException("Start date cannot be in the past.");

                if (request.EndDate < request.StartDate)
                    throw new ArgumentException("End date must be greater than or equal to start date.");

                if (string.IsNullOrWhiteSpace(request.LeaveType))
                    throw new ArgumentException("Leave type is required.");

                var employee = await _leaveRepository.GetEmployeeByUserIdAsync(userId);
                if (employee == null)
                    throw new UnauthorizedAccessException("Employee not found.");

                int leaveDays = (request.EndDate.DayNumber - request.StartDate.DayNumber) + 1;
                if (leaveDays <= 0)
                    throw new ArgumentException("Invalid leave duration.");

                if (employee.LeaveBalance < leaveDays)
                    throw new InvalidOperationException("Insufficient leave balance.");
                var leave = new Leave
                {
                    EmployeeId = employee.EmployeeId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    LeaveType = request.LeaveType,
                    Reason = request.Reason,
                    AppliedAt = DateTime.UtcNow,
                    Status = "Pending"
                };
                await _leaveRepository.ApplyLeaveAsync(leave);
                return "Leave applied successfully.";
            }
            catch (UnauthorizedAccessException ex)
            {
                return ex.Message;
            }
            catch (DataMisalignedException ex)
            {
                return ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return "An unexpected error occurred. Please try again later.";
            }
        }

        public async Task<string> ApproveRejectLeaveAsync(int leaveId, string action, int? adminUserId)
        {
            try
            {
                if (leaveId <= 0)
                    throw new ArgumentException("Invalid leave ID.");

                if (string.IsNullOrWhiteSpace(action))
                    throw new ArgumentException("Action (Approve/Reject) is required.");

                var leave = await _leaveRepository.GetLeaveByIdAsync(leaveId);
                if (leave == null)
                    throw new KeyNotFoundException("Leave request not found.");

                if (!leave.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException($"Leave has already been {leave.Status.ToLower()}.");

                if (action.Equals("Approve", StringComparison.OrdinalIgnoreCase))
                {
                    var employee = await _leaveRepository.GetEmployeeByIdAsync(leave.EmployeeId);
                    if (employee == null)
                        throw new KeyNotFoundException("Employee not found.");

                    int leaveDays = (leave.EndDate.DayNumber - leave.StartDate.DayNumber) + 1;
                    if (leaveDays <= 0)
                        throw new ArgumentException("Invalid leave period.");

                    if (employee.LeaveBalance < leaveDays)
                        throw new InvalidOperationException("Insufficient leave balance.");

                    employee.LeaveBalance -= leaveDays;
                    leave.Status = "Approved";
                    leave.UpdatedAt = DateTime.UtcNow;

                    var empUpdate = await _empRepo.UpdateEmployeeAsync(employee);
                    if (!empUpdate)
                        throw new InvalidOperationException("Failed to update employee leave balance.");
                }
                else if (action.Equals("Reject", StringComparison.OrdinalIgnoreCase))
                {
                    leave.Status = "Rejected";
                    leave.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    throw new ArgumentException("Invalid action. Please use Approve or Reject.");
                }

                await _leaveRepository.UpdateLeaveAsync(leave);
                return $"Leave {leave.Status.ToLower()} successfully.";
            }
            catch (ArgumentException ex)
            {
                return ex.Message;
            }
            catch (KeyNotFoundException ex)
            {
                return ex.Message;
            }
            catch (InvalidOperationException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return "An unexpected error occurred. Please try again later.";
            }
        }
    }
}
