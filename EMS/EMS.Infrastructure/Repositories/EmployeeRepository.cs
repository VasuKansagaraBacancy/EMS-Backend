using EMS.EMS.Application.DTOs.DashboardDTO;
using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Application.DTOs.LeaveDTO;
using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.EMS.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EMSDbContext _context;
        public EmployeeRepository(EMSDbContext context) => _context = context;
        public async Task AddAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Employee.EmployeeId == employeeId);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                _context.Employees.Update(employee);
                var result = await _context.SaveChangesAsync();

                return result > 0; 
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<Employee?> GetEmployeeWithDetailsAsync(int employeeId)
        {
            return await _context.Employees
                .Where(e => e.EmployeeId == employeeId)
                .Include(e => e.User).AsNoTracking()  
                .Include(e => e.Department)  
                .FirstOrDefaultAsync();
        }

        public async Task<List<Timesheet>> GetTimesheetsByEmployeeIdAsync(int employeeId)
        {
            return await _context.Timesheets
                .Where(t => t.EmployeeId == employeeId)
                .OrderByDescending(t => t.Date)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetTotalEmployeesAsync()
        {
            return await _context.Employees.AsNoTracking().CountAsync();
        }

        public async Task<List<ActiveEmployeeDto>> GetMostActiveEmployeesAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _context.Timesheets
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .GroupBy(t => new { t.EmployeeId, t.Employee.User.FirstName, t.Employee.User.LastName })
                .Select(g => new ActiveEmployeeDto
                {
                    EmployeeId = g.Key.EmployeeId,
                    EmployeeName = g.Key.FirstName + " " + g.Key.LastName,
                    TotalHours = (double)g.Sum(t => t.TotalHours)
                })
                .AsNoTracking()
                .OrderByDescending(e => e.TotalHours)
                .ToListAsync();
        }

        public async Task<LeaveAnalyticsDto> GetLeaveAnalyticsAsync()
        {
            var leaveData = await _context.Leaves
                .GroupBy(l => new { l.EmployeeId, l.Employee.User.FirstName, l.Employee.User.LastName })
                .AsNoTracking()
                .Select(g => new EmployeeLeaveDto
                {
                    EmployeeId = g.Key.EmployeeId,
                    EmployeeName = g.Key.FirstName + " " + g.Key.LastName,
                    TotalLeaveDays   = g.Sum(l => EF.Functions.DateDiffDay(l.StartDate, l.EndDate) + 1) // Include both start and end day
                })
                .ToListAsync();

            return new LeaveAnalyticsDto { EmployeeLeaves = leaveData};
        }


        public async Task<Employee?> GetEmployeeByUserIdAsync(int? userId)
        {
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<bool> UpdateEmployeeByEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
