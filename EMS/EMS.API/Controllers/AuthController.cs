using EMS.EMS.Application.DTOs.AuthDTO;
using EMS.EMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace EMS.EMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, IHttpContextAccessor httpContextAccessor, ILogger<AuthController> logger)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                _logger.LogWarning("Login attempt failed due to missing credentials.");
                return BadRequest(new { message = "Invalid login details." });
            }
            try
            {
                _logger.LogInformation("Login attempt for Email: {Email}", dto.Email);
                var result = await _authService.LoginAsync(dto);

                if (string.IsNullOrEmpty(result.Token))
                {
                    _logger.LogWarning("Unauthorized login attempt for Email: {Email}", dto.Email);
                    return Unauthorized(new { message = "Invalid email or password." });
                }
                _logger.LogInformation("Login successful for Email: {Email}", dto.Email);
                return Ok(new { token = result.Token, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login for Email: {Email}", dto.Email);
                return StatusCode(500, new { message = "An error occurred while processing the request.", error = ex.Message });
            }
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    _logger.LogWarning("Logout attempt failed due to missing or invalid token.");
                    return Unauthorized(new { message = "Invalid or missing token." });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();

                if (!handler.CanReadToken(token))
                {
                    _logger.LogWarning("Logout attempt failed due to malformed token.");
                    return BadRequest(new { message = "Malformed token." });
                }

                var jwtToken = handler.ReadJwtToken(token);
                var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");

                if (expClaim == null)
                {
                    _logger.LogWarning("Logout attempt failed: Token does not contain an expiry claim.");
                    return BadRequest(new { message = "Token does not have an expiry date." });
                }

                if (!long.TryParse(expClaim.Value, out var expiryDateUnix))
                {
                    _logger.LogWarning("Logout attempt failed: Invalid token expiry format.");
                    return BadRequest(new { message = "Invalid token expiry format." });
                }

                var expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryDateUnix).UtcDateTime;
                await _authService.LogoutAsync(token, expiryDate);

                _logger.LogInformation("User successfully logged out.");
                return Ok(new { message = "Logout successful." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during logout.");
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordWithTokenDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                _logger.LogWarning("Reset password attempt failed due to missing data.");
                return BadRequest(new { message = "Invalid request data." });
            }

            try
            {
                _logger.LogInformation("Password reset attempt for token: {Token}", dto.Token);
                var result = await _authService.ResetPasswordWithTokenAsync(dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Password reset failed for token: {Token}", dto.Token);
                    return BadRequest(new { message = result.Message });
                }

                _logger.LogInformation("Password reset successful for token: {Token}", dto.Token);
                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing password reset request for token: {Token}", dto.Token);
                return StatusCode(500, new { message = "Error processing password reset request.", error = ex.Message });
            }
        }

        [HttpPost("send-reset-link")]
        public async Task<IActionResult> SendPasswordResetLink([FromBody] ResetPasswordDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email))
            {
                _logger.LogWarning("Password reset link request failed due to missing email.");
                return BadRequest(new { message = "Email is required." });
            }
            try
            {
                _logger.LogInformation("Password reset link requested for Email: {Email}", dto.Email);
                var result = await _authService.ResetPasswordAsync(dto);

                if (!result.Success)
                {
                    _logger.LogWarning("Failed to send password reset link for Email: {Email}", dto.Email);
                    return BadRequest(new { message = result.Message });
                }

                _logger.LogInformation("Password reset link sent successfully for Email: {Email}", dto.Email);
                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password reset link for Email: {Email}", dto.Email);
                return StatusCode(500, new { message = "Error sending password reset link.", error = ex.Message });
            }
        }
    }
}