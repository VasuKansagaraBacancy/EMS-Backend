using EMS.EMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EMS.EMS.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        //for  employee core details
        [Authorize(Roles = "Admin")]
        [HttpGet("Employee-Dashboard/{employeeId}")]
        public async Task<IActionResult> GetEmployeeDashboard(int employeeId)
        {
            try
            {
                var dashboard = await _dashboardService.GetEmployeeDashboardAsync(employeeId);
                if (dashboard == null)
                    return NotFound(new { Message = "Employee not found." });

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard for EmployeeId: {EmployeeId}", employeeId);
                return StatusCode(500, new { Message = "An unexpected error occurred while fetching employee dashboard." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-Dashboard")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var dashboard = await _dashboardService.GetAdminDashboardAsync();
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin dashboard.");
                return StatusCode(500, new { Message = "An unexpected error occurred while fetching admin dashboard." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("most-active-Employee")]
        public async Task<IActionResult> GetMostActiveEmployees([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            try
            {
                if (startDate > endDate)
                    return BadRequest(new { Message = "Start date cannot be later than end date." });

                var result = await _dashboardService.GetMostActiveEmployeesAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving most active employees from {StartDate} to {EndDate}", startDate, endDate);
                return StatusCode(500, new { Message = "An unexpected error occurred while fetching active employees data." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("leave-analytics-of-Employee")]
        public async Task<IActionResult> GetLeaveAnalytics()
        {
            try
            {
                var result = await _dashboardService.GetLeaveAnalyticsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving leave analytics.");
                return StatusCode(500, new { Message = "An unexpected error occurred while fetching leave analytics." });
            }
        }
    }
}