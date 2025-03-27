using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.EMS.Infrastructure.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly EMSDbContext _context;

        public LeaveRepository(EMSDbContext context)
        {
            _context = context;
        }

        public async Task ApplyLeaveAsync(Leave leave)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    _context.Leaves.Add(leave);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while applying leave.", ex);
            }
        }
        public async Task<Employee?> GetEmployeeByUserIdAsync(int? userId)
        {
            try
            {
                return await _context.Employees
                    .FirstOrDefaultAsync(e => e.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching employee by user id", ex);
            }
        }

        public async Task<Leave?> GetLeaveByIdAsync(int leaveId)
        {
            return await _context.Leaves.FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        public async Task<bool> UpdateLeaveAsync(Leave leave)
        {
            try
            {
                _context.Leaves.Update(leave);
                 await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<Employee?> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }
        public async Task<int> GetLeaveBalanceAsync(int employeeId)
        {
            return await _context.Leaves
                .Include(d => d.Employee)
                .Where(l => l.EmployeeId == employeeId)
                .SumAsync(l => l.Employee.LeaveBalance);
        }
        public async Task<List<Leave>> GetPendingLeaveRequestsAsync()
        {
            return await _context.Leaves
                .Where(l => l.Status == "Pending")
                .ToListAsync();
        }
    }
}