using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.EMS.Infrastructure.Repositories
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly EMSDbContext _context;
        public TimesheetRepository(EMSDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Timesheet>> GetTimesheetsByEmployeeIdAsync(int employeeId)
        {
            try
            {
                return await _context.Timesheets
                    .Where(t => t.EmployeeId == employeeId)
                    .OrderByDescending(t => t.Date)
                    .AsNoTracking() // Optimized for read-only
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving timesheets for the employee.", ex);
            }
        }
        public async Task<Timesheet?> GetTimesheetByIdAsync(int timesheetId)
        {
            try
            {
                return await _context.Timesheets
                    .AsNoTracking() // No need to track
                    .FirstOrDefaultAsync(t => t.TimesheetId == timesheetId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving timesheet with ID {timesheetId}.", ex);
            }
        }
        public async Task AddTimesheetAsync(Timesheet timesheet)
        {
            try
            {
                await _context.Timesheets.AddAsync(timesheet);
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding timesheet.", ex);
            }
        }
        public async Task UpdateTimesheetAsync(Timesheet timesheet)
        {
            try
            {
                _context.Timesheets.Update(timesheet);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating timesheet.", ex);
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving changes to the database.", ex);
            }
        }
        public async Task<Employee?> GetEmployeeByUserIdAsync(int? userId)
        {
            try
            {
                return await _context.Employees
                    .AsNoTracking() // Read-only, no tracking needed
                    .FirstOrDefaultAsync(e => e.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching employee by user ID.", ex);
            }
        }
        public async Task<Timesheet?> GetTimesheetByEmployeeIdAndDateAsync(int employeeId, DateOnly date)
        {
            try
            {
                return await _context.Timesheets
                    .AsNoTracking() // Read-only
                    .FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.Date == date);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving timesheet for the given date.", ex);
            }
        }
        public async Task<double> GetTotalLoggedHoursAsync(int employeeId)
        {
            try
            {
                return (double)await _context.Timesheets
                    .Where(t => t.EmployeeId == employeeId)
                    .AsNoTracking() // No modification, so no tracking needed
                    .SumAsync(t => t.TotalHours);
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculating total logged hours.", ex);
            }
        }
        public async Task<List<Timesheet>> GetLatestTimesheetsAsync(int employeeId, int count)
        {
            try
            {
                return await _context.Timesheets
                    .Where(t => t.EmployeeId == employeeId)
                    .OrderByDescending(t => t.Date)
                    .Take(count)
                    .AsNoTracking() 
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving latest timesheets.", ex);
            }
        }
    }
}