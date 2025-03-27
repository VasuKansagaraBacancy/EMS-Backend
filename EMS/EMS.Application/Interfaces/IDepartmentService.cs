using EMS.EMS.Application.DTOs.DepartmentDTO;

namespace EMS.EMS.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync();
        Task<DepartmentResponseDto?> GetDepartmentByIdAsync(int id);
        Task<(bool isSuccess, string message)> CreateDepartmentAsync(DepartmentCreateDto dto);
        Task<(bool isSuccess, string message)> UpdateDepartmentAsync(DepartmentUpdateDto dto);
        Task<(bool isSuccess, string message)> DeleteDepartmentAsync(int id);
    }
}