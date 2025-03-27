using EMS.EMS.Application.DTOs;
using EMS.EMS.Application.DTOs.DashboardDTO;
using EMS.EMS.Application.DTOs.TimeSheetDTO;
using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Interfaces;

namespace EMS.EMS.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly ILeaveRepository _leaveRepository;
        public DashboardService(
            IEmployeeRepository employeeRepository,
            ITimesheetRepository timesheetRepository,
            ILeaveRepository leaveRepository
          )
        {
            _employeeRepository = employeeRepository;
            _timesheetRepository = timesheetRepository;
            _leaveRepository = leaveRepository;
        }
        public async Task<EmployeeDashboardDto> GetEmployeeDashboardAsync(int employeeId)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
                if (employee == null) return null;

                var totalHours = await _timesheetRepository.GetTotalLoggedHoursAsync(employeeId);
                var leaveBalance = await _leaveRepository.GetLeaveBalanceAsync(employeeId);
                var latestTimesheets = await _timesheetRepository.GetLatestTimesheetsAsync(employeeId, 2);

                return new EmployeeDashboardDto
                {
                    EmployeeId = employee.Employee.EmployeeId,
                    Name = employee.FirstName,
                    Email = employee.Email,
                    TotalHoursLogged = (double)totalHours,
                    LeaveBalance = leaveBalance,
                    LatestTimesheets = latestTimesheets.Select(t => new TimesheetDashDto
                    {
                        Date = t.Date,
                        HoursWorked = (double)t.TotalHours
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching employee dashboard.", ex);
            }
        }
        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            try
            {
                var totalEmployees = await _employeeRepository.GetTotalEmployeesAsync();
                var pendingLeaves = await _leaveRepository.GetPendingLeaveRequestsAsync();

                return new AdminDashboardDto
                {
                    TotalEmployees = totalEmployees,
                    PendingLeaveRequests = pendingLeaves.Select(l => new LeaveRequestDashDto
                    {
                        EmployeeId = l.EmployeeId,
                        LeaveType = l.LeaveType,
                        StartDate = l.StartDate,
                        EndDate = l.EndDate
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching admin dashboard.", ex);
            }
        }
        public async Task<List<ActiveEmployeeDto>> GetMostActiveEmployeesAsync(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                return await _employeeRepository.GetMostActiveEmployeesAsync(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching active employees data.", ex);
            }
        }
        public async Task<LeaveAnalyticsDto> GetLeaveAnalyticsAsync()
        {
            try
            {
                return await _employeeRepository.GetLeaveAnalyticsAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while fetching leave analytics.", ex);
            }
        }
    }
}