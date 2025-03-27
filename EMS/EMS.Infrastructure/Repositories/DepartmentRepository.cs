using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.EMS.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly EMSDbContext _context;

        public DepartmentRepository(EMSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsWithEmployeesAsync()
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Employees)
                        .ThenInclude(e => e.User)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                return Enumerable.Empty<Department>(); // Return empty list on failure
            }
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            try
            {
                return await _context.Departments
                    .Include(d => d.Employees)
                        .ThenInclude(e => e.User)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.DepartmentId == id);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> DepartmentExistsAsync(string departmentName, int? excludeId = null)
        {
            try
            {
                return await _context.Departments
                    .AsNoTracking()
                    .AnyAsync(d =>
                    d.DepartmentName.ToLower() == departmentName.ToLower() &&
                    (!excludeId.HasValue || d.DepartmentId != excludeId.Value));
            }
            catch (Exception ex)
            {
                return false; // Assume department doesn't exist to prevent duplicate insert issues
            }
        }
        public async Task<bool> AddDepartmentAsync(Department department)
        {
            try
            {
                await _context.Departments.AddAsync(department);
                return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> UpdateDepartment(Department department)
        {
            try
            {
                _context.Departments.Update(department);
                return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> DeleteDepartment(Department department)
        {
            try
            {
                _context.Departments.Remove(department);
                return await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
