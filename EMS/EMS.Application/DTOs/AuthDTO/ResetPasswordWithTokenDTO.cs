using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Application.DTOs.AuthDTO
{
    public class ResetPasswordWithTokenDTO
    {
        [MaxLength(255)]
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}