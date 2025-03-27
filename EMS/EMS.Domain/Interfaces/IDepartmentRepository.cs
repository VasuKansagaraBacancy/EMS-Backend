using EMS.EMS.Domain.Entities;

namespace EMS.EMS.Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllDepartmentsWithEmployeesAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<bool> AddDepartmentAsync(Department department);
        Task<bool> UpdateDepartment(Department department);
        Task<bool> DeleteDepartment(Department department);
        Task<bool> DepartmentExistsAsync(string departmentName, int? excludeId = null);
    }
}