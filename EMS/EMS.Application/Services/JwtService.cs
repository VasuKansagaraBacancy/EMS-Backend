using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EMS.EMS.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration ;
        }
        public string GenerateToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User data must be provided to generate a token.");
            try
            {
                string roleName = user.RoleId == 1 ? "Admin" : "Employee";

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim("RoleId", user.RoleId.ToString()),
                    new Claim("RoleName", roleName)
                };
                // Fetch JWT settings securely
                var key = _configuration["JwtSettings:Key"] ?? throw new Exception("JWT Key is missing.");
                var issuer = _configuration["JwtSettings:Issuer"] ?? throw new Exception("JWT Issuer is missing.");
                var audience = _configuration["JwtSettings:Audience"] ?? throw new Exception("JWT Audience is missing.");
                var durationInMinutes = _configuration["JwtSettings:DurationInMinutes"];

                if (string.IsNullOrEmpty(durationInMinutes) || !double.TryParse(durationInMinutes, out double expiryMinutes))
                {
                    throw new Exception("Invalid JWT duration configuration.");
                }
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while generating the JWT token.", ex);
            }
        }
    }
}