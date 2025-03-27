using EMS.EMS.Application.DTOs.ReportDTO;
using EMS.EMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EMS.EMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _service;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportsService service, ILogger<ReportsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("employee-work-hours-report")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetEmployeeWorkHoursReport([FromBody] ReportRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    _logger.LogWarning("Received null request for employee work hours report.");
                    return BadRequest(new { Message = "Invalid request data." });
                }

                var reports = await _service.GetEmployeeWorkHoursReportAsync(request);
                return Ok(reports);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Invalid request data for employee work hours report.");
                return BadRequest(new { Message = argEx.Message });
            }
            catch (UnauthorizedAccessException uaEx)
            {
                _logger.LogWarning(uaEx, "Unauthorized access attempt to employee work hours report.");
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while generating employee work hours report.");
                return StatusCode(500, new { Message = "An unexpected error occurred. Please try again later." });
            }
        }
    }
}
