using EMS.EMS.Application.DTOs.AdminDTO;
using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Shared.Helpers;

namespace EMS.EMS.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordResetTokenRepo _passwordResetTokenRepo;
        private readonly IEmailService _emailService;
        public AdminService(IUserRepository userRepo, IPasswordResetTokenRepo passwordResetTokenRepo, IEmailService emailService)
        {
            _userRepo = userRepo;
            _passwordResetTokenRepo = passwordResetTokenRepo;
            _emailService = emailService;
        }
        public async Task<(bool Success, string Message)> CreateAdminAsync(CreateAdminDTO dto, int createdBy)
        {
            try
            {
                if (await _userRepo.ExistsByEmail(dto.Email))
                    return (false, "Email already exists.");

                var password = PasswordHelper.GeneratePassword(6);
                var hashed = PasswordHelper.HashPassword(password);

                var user = new User
                {
                    RoleId = 1,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Password = hashed,
                    CreatedBy = createdBy,
                    UpdatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _userRepo.AddAsync(user);

                var resetToken = new PasswordResetToken
                {
                    UserId = user.UserId,
                    Token = Guid.NewGuid().ToString(),
                    ExpiryDate = DateTime.UtcNow.AddHours(1),
                    IsUsed = false
                };

                await _passwordResetTokenRepo.AddAsync(resetToken);

                string resetLink = $"https://localhost:7006/swagger/index.html#/api/user/reset-password?token={resetToken.Token}";

                await _emailService.SendEmailUser(user, resetLink);

                return (true, "Admin created successfully. Reset password link has been sent to the registered email.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while creating the Admin: {ex.Message}");
            }
        }
        public async Task<List<AdminResponseDTO>> GetAllAdminsAsync()
        {
            try
            {
                var admins = await _userRepo.GetAllAdminsAsync();
                return admins ?? new List<AdminResponseDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching Admins.", ex);
            }
        }
        public async Task<UserProfileDTO> GetLoggedInUserProfileAsync(int? userId, int? roleId)
        {
            try
            {
                if (userId == null || userId <= 0)
                    throw new ArgumentException("Invalid User Id.");

                var user = await _userRepo.GetUserWithDetailsByIdAsync(userId);

                if (user == null)
                    throw new Exception("User not found or inactive.");

                var dto = new UserProfileDTO
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    RoleId = user.RoleId,
                    RoleName = roleId == 1 ? "Admin" : "Employee"
                };

                if (roleId == 2 && user.Employee != null)
                {
                    dto.EmployeeDetails = new EmployeeProfileDTO
                    {
                        EmployeeId = user.Employee.EmployeeId,
                        DepartmentName = user.Employee.Department?.DepartmentName,
                        TechStack = user.Employee.TechStack,
                        DateOfBirth = user.Employee.DateOfBirth.Value,
                        Address = user.Employee.Address,
                        Leavebalance=user.Employee.LeaveBalance
                    };
                }
                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching user profile.", ex);
            }
        }
    }
}