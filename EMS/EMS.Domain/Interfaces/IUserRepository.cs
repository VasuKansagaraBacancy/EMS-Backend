using EMS.EMS.Application.DTOs.AdminDTO;
using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Domain.Entities;

namespace EMS.EMS.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmail(string email);
        Task AddAsync(User user);
        Task<User> GetByEmail(string email);
        Task<User> GetByIdAsync(int userId);
        Task UpdateAsync(User user);
        Task<List<AdminResponseDTO>> GetAllAdminsAsync();
        Task<List<EmployeeResponseDTO>> GetAllEmployeesAsync();
        Task<User?> GetUserWithDetailsByIdAsync(int? userId);
    }
}