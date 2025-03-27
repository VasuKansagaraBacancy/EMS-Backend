using EMS.EMS.Application.DTOs.EmployeeDTO;

namespace EMS.EMS.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<(bool Success, string Message)> CreateEmployeeAsync(EmployeeWithUserDTO dto, int createdBy);
        Task<bool> UpdateEmployeeAsync(int EmployeeId, EmployeeUpdateDTO employeeDto, int? updatedByUserId);
        Task<List<EmployeeResponseDTO>> GetAllEmployeesAsync();
        Task<bool> ToggleEmployeeActivationAsync(int employeeId, bool isActive, int? updatedByUserId);
        Task<UserProfileDTO> GetLoggedInUserProfileAsync(int userId, int roleId);
        Task<EmployeeDetailsWithTimesheetsDto?> GetEmployeeDetailsWithTimesheetsAsync(int employeeId);
        Task<byte[]> ExportTimesheetsToExcelAsync(int employeeId);
        Task<bool> UpdateEmployeeProfileAsync(int? userId, UpdateEmployeeProfileDto dto);
    }
}