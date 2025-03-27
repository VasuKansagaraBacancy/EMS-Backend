using EMS.EMS.Application.DTOs.DepartmentDTO;
using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;

namespace EMS.EMS.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<DepartmentResponseDto>> GetAllDepartmentsAsync()
        {
            try
            {
                var departments = await _repository.GetAllDepartmentsWithEmployeesAsync();

                if (departments == null || !departments.Any())
                    return new List<DepartmentResponseDto>();

                return departments.Select(d => new DepartmentResponseDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    Employees = d.Employees?.Select(e => new EmployeeDepartmentDto
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeName = $"{e.User.FirstName} {e.User.LastName}"
                    }).ToList() ?? new List<EmployeeDepartmentDto>()
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<DepartmentResponseDto?> GetDepartmentByIdAsync(int id)
        {
            try
            {
                var department = await _repository.GetDepartmentByIdAsync(id);
                if (department == null) return null;

                return new DepartmentResponseDto
                {
                    DepartmentId = department.DepartmentId,
                    DepartmentName = department.DepartmentName,
                    Employees = department.Employees?.Select(e => new EmployeeDepartmentDto
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeName = $"{e.User.FirstName} {e.User.LastName}"
                    }).ToList() ?? new List<EmployeeDepartmentDto>()
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<(bool isSuccess, string message)> CreateDepartmentAsync(DepartmentCreateDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.DepartmentName))
                    throw new ArgumentException("Department name is required.");

                var exists = await _repository.DepartmentExistsAsync(dto.DepartmentName);
                if (exists)
                    return (false, "Department with this name already exists.");

                var department = new Department
                {
                    DepartmentName = dto.DepartmentName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }; 
                var result = await _repository.AddDepartmentAsync(department);
                return result ? (true, "Department created successfully.") : (false, "Failed to create department.");
            }
            catch (ArgumentException ex)
            {
                return (false, ex.Message);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool isSuccess, string message)> UpdateDepartmentAsync(DepartmentUpdateDto dto)
        {
            try
            {
                var department = await _repository.GetDepartmentByIdAsync(dto.DepartmentId);
                if (department == null)
                    return (false, "Department not found.");

                var exists = await _repository.DepartmentExistsAsync(dto.DepartmentName, dto.DepartmentId);
                if (exists)
                    return (false, "Another department with this name already exists.");

                department.DepartmentName = dto.DepartmentName;
                department.UpdatedAt = DateTime.UtcNow;
                
                var result = await _repository.UpdateDepartment(department);
                return result ? (true, "Department updated successfully.") : (false, "Failed to update department.");
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while updating the department.");
            }
        }
        public async Task<(bool isSuccess, string message)> DeleteDepartmentAsync(int id)
        {
            try
            {
                var department = await _repository.GetDepartmentByIdAsync(id);
                if (department == null)
                    return (false, "Department not found.");

                if (department.Employees != null && department.Employees.Any())
                    return (false, "Cannot delete department with assigned employees.");       
                var result = await _repository.DeleteDepartment(department);
                return result ? (true, "Department deleted successfully.") : (false, "Failed to delete department.");
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while deleting the department.");
            }
        }
    }
}