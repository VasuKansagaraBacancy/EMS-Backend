using EMS.EMS.Application.DTOs.AdminDTO;
using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMS.EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<UserController> _logger;
        public UserController(IAdminService adminService, IEmployeeService employeeService, ILogger<UserController> logger)
        {
            _adminService = adminService;
            _employeeService = employeeService;
            _logger = logger; // Assign logger
        }

        [HttpPost("CreateAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDTO dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Unauthorized access attempt to CreateAdmin. Missing userId in token.");
                    return Unauthorized(new { message = "Invalid token claims." });
                }

                _logger.LogInformation("Admin creation requested by UserId {UserId}", userId.Value);
                var result = await _adminService.CreateAdminAsync(dto, userId.Value);

                if (result.Success)
                {
                    _logger.LogInformation("Admin created successfully by UserId {UserId}", userId.Value);
                    return Created(string.Empty, new { message = result.Message });
                }
                else
                {
                    _logger.LogWarning("Admin creation failed: {Message}", result.Message);
                    return Conflict(new { message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an admin.");
                return HandleServerError(ex);
            }
        }

        [HttpPost("CreateEmployee")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeWithUserDTO dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Unauthorized access attempt to CreateEmployee. Missing userId in token.");
                    return Unauthorized(new { message = "Invalid token claims." });
                }

                _logger.LogInformation("Employee creation requested by UserId {UserId}", userId.Value);
                var result = await _employeeService.CreateEmployeeAsync(dto, userId.Value);

                if (result.Success)
                {
                    _logger.LogInformation("Employee created successfully by UserId {UserId}", userId.Value);
                    return Created(string.Empty, new { message = result.Message });
                }
                else
                {
                    _logger.LogWarning("Employee creation failed: {Message}", result.Message);
                    return Conflict(new { message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating an employee.");
                return HandleServerError(ex);
            }
        }


        [HttpGet("Get-All-Admins")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdmins()
        {
            try
            {
                _logger.LogInformation("Fetching all admins requested.");

                var admins = await _adminService.GetAllAdminsAsync();

                if (admins.Any())
                {
                    _logger.LogInformation("Successfully retrieved {AdminCount} admins.", admins.Count());
                    return Ok(admins);
                }
                else
                {
                    _logger.LogWarning("No Admin users found.");
                    return NotFound(new { message = "No Admin users found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving admins.");
                return HandleServerError(ex);
            }
        }

        [HttpGet("Get-All-employees")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                _logger.LogInformation("Fetching all employees requested.");

                var employees = await _employeeService.GetAllEmployeesAsync();

                if (employees.Any())
                {
                    _logger.LogInformation("Successfully retrieved {EmployeeCount} employees.", employees.Count());
                    return Ok(employees);
                }
                else
                {
                    _logger.LogWarning("No Employee users found.");
                    return NotFound(new { message = "No Employee users found." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving employees.");
                return HandleServerError(ex);
            }
        }

        [HttpPut("update-Employee-By-Admin/{empId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(int empId, [FromBody] EmployeeUpdateDTO employeeDto)
        {
            try
            {
                _logger.LogInformation("Update request received for Employee ID: {EmpId}", empId);

                var userId = GetUserIdFromClaims();
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Unauthorized access attempt. Invalid user token.");
                    return Unauthorized(new { message = "Invalid user token." });
                }

                var result = await _employeeService.UpdateEmployeeAsync(empId, employeeDto, userId.Value);

                if (result)
                {
                    _logger.LogInformation("Employee ID: {EmpId} updated successfully by User ID: {UserId}.", empId, userId.Value);
                    return Ok(new { message = "Employee updated successfully." });
                }
                else
                {
                    _logger.LogWarning("Update failed for Employee ID: {EmpId}.", empId);
                    return BadRequest(new { message = "Update failed." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating Employee ID: {EmpId}.", empId);
                return HandleServerError(ex);
            }
        }


        [HttpPut("activation-deactivation")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleEmployeeActivation([FromQuery] int employeeId, [FromQuery] bool isActive)
        {
            try
            {
                _logger.LogInformation("Received request to {Action} Employee ID: {EmployeeId}.", isActive ? "activate" : "deactivate", employeeId);

                var updatedByUserId = GetUserIdFromClaims();
                if (!updatedByUserId.HasValue)
                {
                    _logger.LogWarning("Unauthorized access attempt for employee activation. Invalid user token.");
                    return Unauthorized(new { message = "Invalid user token." });
                }

                if (employeeId <= 0)
                {
                    _logger.LogWarning("Invalid Employee ID: {EmployeeId} provided for activation.", employeeId);
                    return BadRequest(new { message = "Invalid employee ID." });
                }

                var result = await _employeeService.ToggleEmployeeActivationAsync(employeeId, isActive, updatedByUserId.Value);

                if (result)
                {
                    _logger.LogInformation("Employee ID: {EmployeeId} was successfully {Action}d by User ID: {UpdatedByUserId}.",
                        employeeId, isActive ? "activate" : "deactivate", updatedByUserId.Value);
                    return Ok(new { message = $"Employee {(isActive ? "activated" : "deactivated")} successfully." });
                }
                else
                {
                    _logger.LogWarning("Action failed for Employee ID: {EmployeeId}.", employeeId);
                    return BadRequest(new { message = "Action failed." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while trying to {Action} Employee ID: {EmployeeId}.",
                    isActive ? "activate" : "deactivate", employeeId);
                return HandleServerError(ex);
            }
        }


        [HttpGet("My-profile")]
        [Authorize]
        public async Task<IActionResult> GetLoggedInUserProfile()
        {
            try
            {
                _logger.LogInformation("Fetching profile for the logged-in user.");

                var userId = GetUserIdFromClaims();
                var roleId = GetRoleIdFromClaims();

                if (!userId.HasValue || !roleId.HasValue)
                {
                    _logger.LogWarning("Unauthorized access: Invalid token claims for user profile.");
                    return Unauthorized(new { message = "Invalid token." });
                }

                _logger.LogInformation("Fetching profile for User ID: {UserId}, Role ID: {RoleId}.", userId.Value, roleId.Value);

                var profile = await _adminService.GetLoggedInUserProfileAsync(userId.Value, roleId.Value);

                if (profile == null)
                {
                    _logger.LogWarning("Profile not found for User ID: {UserId}, Role ID: {RoleId}.", userId.Value, roleId.Value);
                    return NotFound(new { message = "User profile not found." });
                }

                _logger.LogInformation("Successfully retrieved profile for User ID: {UserId}.", userId.Value);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the profile for User ");
                return HandleServerError(ex);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("{employeeId}/details-with-timesheets")]
        public async Task<IActionResult> GetEmployeeDetailsWithTimesheets(int employeeId)
        {
            try
            {
                _logger.LogInformation("Fetching details and timesheets for Employee ID: {EmployeeId}", employeeId);

                var employeeDetails = await _employeeService.GetEmployeeDetailsWithTimesheetsAsync(employeeId);

                if (employeeDetails == null)
                {
                    _logger.LogWarning("No details or timesheets found for Employee ID: {EmployeeId}", employeeId);
                    return NotFound(new { message = "Employee not found or has no timesheets." });
                }

                _logger.LogInformation("Successfully retrieved details and timesheets for Employee ID: {EmployeeId}", employeeId);
                return Ok(employeeDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving details and timesheets for Employee ID: {EmployeeId}", employeeId);
                return HandleServerError(ex);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{employeeId}/export-timesheets")]
        public async Task<IActionResult> ExportTimesheets(int employeeId)
        {
            try
            {
                _logger.LogInformation("Exporting timesheets for Employee ID: {EmployeeId}", employeeId);

                var fileBytes = await _employeeService.ExportTimesheetsToExcelAsync(employeeId);

                if (fileBytes == null)
                {
                    _logger.LogWarning("No timesheets found for Employee ID: {EmployeeId}", employeeId);
                    return NotFound(new { message = "Employee not found or has no timesheets." });
                }

                _logger.LogInformation("Successfully exported timesheets for Employee ID: {EmployeeId}", employeeId);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Timesheets_{employeeId}.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while exporting timesheets for Employee ID: {EmployeeId}", employeeId);
                return HandleServerError(ex);
            }
        }

        //For Employee Update
        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateEmployeeProfile([FromBody] UpdateEmployeeProfileDto dto)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (!userId.HasValue)
                {
                    _logger.LogWarning("Unauthorized access attempt for profile update.");
                    return Unauthorized(new { message = "Invalid user token." });
                }

                _logger.LogInformation("User ID {UserId} is attempting to update their profile.", userId.Value);

                var result = await _employeeService.UpdateEmployeeProfileAsync(userId.Value, dto);

                if (result)
                {
                    _logger.LogInformation("Profile updated successfully for User ID: {UserId}", userId.Value);
                    return Ok(new { message = "Profile updated successfully." });
                }
                else
                {
                    _logger.LogWarning("Profile update failed for User ID: {UserId}", userId.Value);
                    return BadRequest(new { message = "Failed to update profile." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the profile for User ");
                return HandleServerError(ex);
            }
        }
        #region Private Helpers
        private int? GetUserIdFromClaims()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out var userId) ? userId : null;
        }
        private int? GetRoleIdFromClaims()
        {
            var roleIdClaim = User.Claims.FirstOrDefault(c => c.Type == "RoleId");
            return int.TryParse(roleIdClaim?.Value, out var roleId) ? roleId : null;
        }

        private ObjectResult HandleServerError(Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An unexpected error occurred.", error = ex.Message });
        }
        #endregion
    }
}
