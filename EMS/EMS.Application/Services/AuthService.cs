using EMS.EMS.Application.DTOs.AuthDTO;
using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Shared.Helpers;

namespace EMS.EMS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenBlacklistRepository _tokenBlacklistRepository;
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _jwtService;
        private readonly IPasswordResetTokenRepo _passwordResetTokenRepo;
        private readonly IEmailService _emailService;
        public AuthService(
            ITokenBlacklistRepository tokenBlacklistRepository,
            IUserRepository userRepo,
            IJwtService jwtService,
            IPasswordResetTokenRepo passwordResetTokenRepo,
            IEmailService emailService)
        {
            _tokenBlacklistRepository = tokenBlacklistRepository;
            _userRepo = userRepo;
            _jwtService = jwtService;
            _passwordResetTokenRepo = passwordResetTokenRepo;
            _emailService = emailService;
        }
        public async Task<(bool Success, string Message ,string? Token)> LoginAsync(LoginDTO dto)
        {
            try
            {
                var user = await _userRepo.GetByEmail(dto.Email);
                if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.Password))
                    return (false, "Invalid credentials.",  null);

                var token = _jwtService.GenerateToken(user);
                return (true, "Login successful.", token);
            }
            catch (Exception ex)
            {
                return (false, "An error occurred. Please try again.", null);
            }
        }
        public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            try
            {
                var user = await _userRepo.GetByEmail(dto.Email);
                if (user == null)
                    return (false, "No account found with this email.");

                var token = Guid.NewGuid().ToString();
                var expiryDate = DateTime.UtcNow.AddMinutes(30);

                var resetToken = new PasswordResetToken
                {
                    UserId = user.UserId,
                    Token = token,
                    ExpiryDate = expiryDate,
                    IsUsed = false
                };
                await _passwordResetTokenRepo.AddAsync(resetToken);

                string resetLink = $"https://localhost:7006/api/Auth/reset-password?token={resetToken.Token}";
                await _emailService.ResetPasswordEmail(user, resetLink);

                return (true, "Password reset link has been sent.");
            }
            catch (Exception ex)
            {
                return (false, "An error occurred.");
            }
        }
        public async Task<(bool Success, string Message )> ResetPasswordWithTokenAsync(ResetPasswordWithTokenDTO dto)
        {
            try
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                    return (false, "Passwords do not match.");

                var resetToken = await _passwordResetTokenRepo.GetByTokenAsync(dto.Token);
                if (resetToken == null || resetToken.IsUsed || resetToken.ExpiryDate < DateTime.UtcNow)
                    return (false, "Invalid or expired token.");

                var user = await _userRepo.GetByIdAsync(resetToken.UserId);
                if (user == null)
                    return (false, "User not found.");

                user.Password = PasswordHelper.HashPassword(dto.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = user.UserId;
                await _userRepo.UpdateAsync(user);

                resetToken.IsUsed = true;
                await _passwordResetTokenRepo.UpdateAsync(resetToken);

                return (true, "Password reset successfully.");
            }
            catch (Exception ex)
            {
                return (false, "An error occurred.");
            }
        }
        public async Task<(bool Success, string Message)> LogoutAsync(string token, DateTime expiryDate)
        {
            try
            {
                var blacklistToken = new TokenBlacklist
                {
                    Token = token,
                    ExpiryDate = expiryDate
                };

                await _tokenBlacklistRepository.AddTokenAsync(blacklistToken);
                return (true, "Logout successful.");
            }
            catch (Exception ex)
            {
                return (false, "An error occurred.");
            }
        }
    }
}