using EMS.EMS.Application.DTOs.AdminDTO;
using EMS.EMS.Application.DTOs.EmployeeDTO;

namespace EMS.EMS.Application.Interfaces
{
    public interface IAdminService
    {
        Task<(bool Success, string Message)> CreateAdminAsync(CreateAdminDTO dto, int createdBy);
        Task<List<AdminResponseDTO>> GetAllAdminsAsync();
        Task<UserProfileDTO> GetLoggedInUserProfileAsync(int? userId, int? roleId);
    }
}