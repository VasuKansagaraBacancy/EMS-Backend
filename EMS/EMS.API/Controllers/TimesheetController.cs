using EMS.EMS.Application.DTOs.TimeSheetDTO;
using EMS.EMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EMS.EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TimesheetController : ControllerBase
    {
        private readonly ITimesheetService _service;
        private readonly ILogger<TimesheetController> _logger;

        public TimesheetController(ITimesheetService service, ILogger<TimesheetController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("MyTimeSheets")]
        public async Task<IActionResult> GetTimesheetsByEmployee()
        {
            var userId = GetUserIdFromToken();
            if (!userId.HasValue)
            {
                _logger.LogWarning("Unauthorized access attempt to MyTimeSheet endpoint.");
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            try
            {
                _logger.LogInformation("Fetching timesheets for UserId: {UserId}", userId);
                var timesheets = await _service.GetTimesheetsForEmployeeAsync(userId.Value);
                return Ok(timesheets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching timesheets for UserId: {UserId}", userId);
                return StatusCode(500, new { message = "Error retrieving timesheets.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("Time-Sheet-By-Id{id}")]
        public async Task<IActionResult> GetTimesheetById(int id)
        {
            var userId = GetUserIdFromToken();
            if (!userId.HasValue)
            {
                _logger.LogWarning("Unauthorized access attempt to GetTimesheetById endpoint.");
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            try
            {
                _logger.LogInformation("Fetching timesheet with ID {TimesheetId} for UserId {UserId}", id, userId);
                var timesheet = await _service.GetTimesheetByIdAsync(id, userId.Value);
                if (timesheet == null)
                {
                    _logger.LogWarning("Timesheet with ID {TimesheetId} not found for UserId {UserId}", id, userId);
                    return NotFound(new { message = $"Timesheet with ID {id} not found." });
                }

                return Ok(timesheet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching timesheet with ID {TimesheetId} for UserId {UserId}", id, userId);
                return StatusCode(500, new { message = "Error retrieving timesheet.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("Add-Timesheet")]
        public async Task<IActionResult> AddTimesheet([FromBody] TimesheetRequestDto dto)
        {
            var userId = GetUserIdFromToken();
            if (!userId.HasValue)
            {
                _logger.LogWarning("Unauthorized access attempt to AddTimesheet endpoint.");
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            if (dto == null)
            {
                _logger.LogWarning("Received null payload in AddTimesheet request.");
                return BadRequest(new { message = "Invalid request payload." });
            }

            try
            {
                _logger.LogInformation("Adding new timesheet for UserId {UserId}", userId);
                var result = await _service.AddTimesheetAsync(dto, userId.Value);
                if (result)
                {
                    _logger.LogInformation("Successfully added timesheet for UserId {UserId}", userId);
                    return CreatedAtAction(nameof(GetTimesheetsByEmployee), new { userId = userId.Value }, new { message = "Timesheet added successfully." });
                }

                _logger.LogWarning("Failed to add timesheet for UserId {UserId}", userId);
                return BadRequest(new { message = "Failed to add timesheet." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding timesheet for UserId {UserId}", userId);
                return StatusCode(500, new { message = "Error adding timesheet.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("Update-Timesheet{id}")]
        public async Task<IActionResult> UpdateTimesheet(int id, [FromBody] TimesheetUpdateRequestDto dto)
        {
            var userId = GetUserIdFromToken();
            if (!userId.HasValue)
            {
                _logger.LogWarning("Unauthorized access attempt to UpdateTimesheet endpoint.");
                return Unauthorized(new { message = "Invalid or missing user ID in token." });
            }

            if (dto == null)
            {
                _logger.LogWarning("Received null payload in UpdateTimesheet request.");
                return BadRequest(new { message = "Invalid request payload." });
            }

            try
            {
                _logger.LogInformation("Updating timesheet with ID {TimesheetId} for UserId {UserId}", id, userId);
                var result = await _service.UpdateTimesheetAsync(dto, userId.Value);
                if (result)
                {
                    _logger.LogInformation("Successfully updated timesheet with ID {TimesheetId} for UserId {UserId}", id, userId);
                    return Ok(new { message = "Timesheet updated successfully." });
                }

                _logger.LogWarning("Failed to update timesheet with ID {TimesheetId} for UserId {UserId}", id, userId);
                return BadRequest(new { message = "Failed to update timesheet." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating timesheet with ID {TimesheetId} for UserId {UserId}", id, userId);
                return StatusCode(500, new { message = "Error updating timesheet.", error = ex.Message });
            }
        }
        #region Private Helpers
        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return null;
            }

            return int.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }
        #endregion
    }
}
