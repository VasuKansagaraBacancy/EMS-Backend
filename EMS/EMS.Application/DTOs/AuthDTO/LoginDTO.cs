﻿using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Application.DTOs.AuthDTO
{
    public class LoginDTO
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}