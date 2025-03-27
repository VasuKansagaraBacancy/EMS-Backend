using EMS.EMS.Application.DTOs.LeaveDTO;
using EMS.EMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EMS.EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;
        private readonly ILogger<LeaveController> _logger;

        public LeaveController(ILeaveService leaveService, ILogger<LeaveController> logger)
        {
            _leaveService = leaveService;
            _logger = logger;
        }

        [HttpPost("apply-Leave")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ApplyLeave([FromBody] LeaveRequestDTO request)
        {
            try
            {
                var employeeId = GetUserIdFromClaims();
                if (employeeId == null)
                    return Unauthorized(new { message = "Invalid user or token." });

                if (request == null)
                    return BadRequest(new { message = "Request body cannot be null." });

                var result = await _leaveService.ApplyForLeaveAsync(request, employeeId.Value);

                return result.Contains("successfully", StringComparison.OrdinalIgnoreCase)
                    ? Ok(new { message = result })
                    : BadRequest(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while applying for leave.");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpPost("approve-reject-leave")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRejectLeave(int leaveId, string action)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (userId == null)
                    return Unauthorized(new { message = "Invalid token or unauthorized access." });

                if (leaveId <= 0)
                    return BadRequest(new { message = "Invalid leave ID." });

                if (string.IsNullOrWhiteSpace(action))
                    return BadRequest(new { message = "Action (approve/reject) is required." });

                var result = await _leaveService.ApproveRejectLeaveAsync(leaveId, action, userId.Value);

                return result.Contains("successfully", StringComparison.OrdinalIgnoreCase)
                    ? Ok(new { message = result })
                    : BadRequest(new { message = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while approving/rejecting leave.");
                return StatusCode(500, new { message = "An unexpected error occurred. Please try again later." });
            }
        }

        #region Private Helpers
        private int? GetUserIdFromClaims()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim?.Value, out var userId) ? userId : null;
        }
        #endregion
    }
}
