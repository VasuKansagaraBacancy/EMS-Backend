using EMS.EMS.Application.DTOs.AdminDTO;
using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EMS.EMS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EMSDbContext _context;
        public UserRepository(EMSDbContext context) => _context = context;
        public async Task<bool> ExistsByEmail(string email)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception("Database error while checking email existence", ex);
            }
        }
        public async Task AddAsync(User user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding user", ex);
            }
        }
        public async Task<User?> GetByEmail(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user by email", ex);
            }
        }
        public async Task<User?> GetByIdAsync(int userId)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user by ID", ex);
            }
        }
        public async Task UpdateAsync(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating user", ex);
            }
        }
        public async Task<List<AdminResponseDTO>> GetAllAdminsAsync()
        {
            try
            {
                return await _context.Users
                    .Where(u => u.RoleId == 1 && u.IsActive)
                    .Select(u => new AdminResponseDTO
                    {
                        UserId = u.UserId,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        Phone = u.Phone
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving admins", ex);
            }
        }
        public async Task<List<EmployeeResponseDTO>> GetAllEmployeesAsync()
        {
            try
            {
                return await _context.Users
                   .Where(u => u.RoleId == 2 && u.IsActive)
                   .Include(d => d.Employee)
                       .ThenInclude(d => d.Department)
                   .Select(u => new EmployeeResponseDTO
                   {
                       UserId = u.UserId,
                       EmployeeId = u.Employee.EmployeeId,
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       DepartmentName = u.Employee.Department.DepartmentName,
                       TechStack = u.Employee.TechStack,
                       Email = u.Email,
                       Phone = u.Phone,
                       DateOfBirth = u.Employee.DateOfBirth,
                       Address = u.Employee.Address,
                       LeaveBalance=u.Employee.LeaveBalance,
                   })
                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving employees", ex);
            }
        }
        public async Task<User?> GetUserWithDetailsByIdAsync(int? userId)
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Employee)
                        .ThenInclude(e => e.Department)
                    .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving user with details", ex);
            }
        }
    }
}