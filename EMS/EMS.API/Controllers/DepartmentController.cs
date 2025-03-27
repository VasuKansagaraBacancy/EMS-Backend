using EMS.EMS.Application.DTOs.DepartmentDTO;
using EMS.EMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;
        private readonly ILogger<DepartmentController> _logger;
        public DepartmentController(IDepartmentService service, ILogger<DepartmentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("Get-All-Departments")]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                var departments = await _service.GetAllDepartmentsAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all departments.");
                return StatusCode(500, new { message = "An error occurred while fetching departments." });
            }
        }

        [HttpGet("Get-All-Departments-By-Id{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            try
            {
                var department = await _service.GetDepartmentByIdAsync(id);
                if (department == null)
                    return NotFound(new { message = "Department not found." });

                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching department with ID {id}.");
                return StatusCode(500, new { message = "An error occurred while fetching the department." });
            }
        }

        [HttpPost("Create-Department")]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentCreateDto dto)
        {
            try
            {
                var (isSuccess, message) = await _service.CreateDepartmentAsync(dto);
                if (!isSuccess) return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating a department.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating a new department.");
                return StatusCode(500, new { message = "An error occurred while creating the department." });
            }
        }

        [HttpPut("Update-Department-By-Id{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentUpdateDto dto)
        {
            try
            {
                if (id != dto.DepartmentId)
                    return BadRequest(new { message = "ID mismatch." });

                var (isSuccess, message) = await _service.UpdateDepartmentAsync(dto);
                if (!isSuccess) return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating a department.");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating department with ID {id}.");
                return StatusCode(500, new { message = "An error occurred while updating the department." });
            }
        }

        [HttpDelete("Delete-Department-By-Id{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var (isSuccess, message) = await _service.DeleteDepartmentAsync(id);
                if (!isSuccess) return BadRequest(new { message });

                return Ok(new { message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting department with ID {id}.");
                return StatusCode(500, new { message = "An error occurred while deleting the department." });
            }
        }
    }
}