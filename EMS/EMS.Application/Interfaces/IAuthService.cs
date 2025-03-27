using EMS.EMS.Application.DTOs.AuthDTO;
using System.Net;

namespace EMS.EMS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, string? Token)> LoginAsync(LoginDTO dto);
        Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordDTO dto);
        Task<(bool Success, string Message)> ResetPasswordWithTokenAsync(ResetPasswordWithTokenDTO dto);
        Task<(bool Success, string Message)> LogoutAsync(string token, DateTime expiryDate);
    }
}