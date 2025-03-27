using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Application.DTOs.AuthDTO
{
    public class ResetPasswordDTO
    {
        [EmailAddress]
        public required string Email { get; set; }
    }
}