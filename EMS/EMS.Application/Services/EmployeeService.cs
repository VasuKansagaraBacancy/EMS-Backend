using ClosedXML.Excel;
using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Application.DTOs.TimeSheetDTO;
using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Shared.Helpers;

namespace EMS.EMS.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUserRepository _userRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IPasswordResetTokenRepo _passwordResetTokenRepo;
        private readonly IEmailService _emailService;
        private readonly ITimesheetRepository _timesheetRepository;
        public EmployeeService(IUserRepository userRepo, IEmployeeRepository employeeRepo, IPasswordResetTokenRepo passwordResetTokenRepo, IEmailService emailService, ITimesheetRepository timesheetRepository)
        {
            _userRepo = userRepo;
            _employeeRepo = employeeRepo;
            _passwordResetTokenRepo=passwordResetTokenRepo;
            _emailService=emailService;
            _timesheetRepository = timesheetRepository;
        }
        public async Task<(bool Success, string Message)> CreateEmployeeAsync(EmployeeWithUserDTO dto, int createdBy)
        {
            try
            {
                if (await _userRepo.ExistsByEmail(dto.Email))
                    return (false, "Email already exists.");

                var password = PasswordHelper.GeneratePassword(6);
                var hashed = PasswordHelper.HashPassword(password);

                var user = new User
                {
                    RoleId = 2,
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

                var employee = new Employee
                {
                    UserId = user.UserId,
                    Address = dto.Address,
                    DateOfBirth = dto.DateOfBirth,
                    DepartmentId = dto.DepartmentId,
                    TechStack = dto.TechStack
                };

                await _employeeRepo.AddAsync(employee);

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

                return (true, "Employee created successfully. Reset password link has been sent to the registered email.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while creating the employee: {ex.Message}");
            }
        }
         public async Task<bool> UpdateEmployeeAsync(int EmployeeId, EmployeeUpdateDTO employeeDto, int? updatedByUserId)
         {
            try
            {
                if (employeeDto == null || EmployeeId <= 0)
                    throw new ArgumentException("Invalid employee data.");

                var user = await _employeeRepo.GetEmployeeByIdAsync(EmployeeId);

                if (user == null)
                    throw new Exception("Employee not found.");

                user.FirstName = employeeDto.FirstName ?? user.FirstName;
                user.LastName = employeeDto.LastName ?? user.LastName;
                user.Phone = employeeDto.Phone ?? user.Phone;
                user.Email = employeeDto.Email ?? user.Email;
                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = updatedByUserId;

                if (user.Employee != null)
                {
                    user.Employee.TechStack = employeeDto.TechStack ?? user.Employee.TechStack;
                    user.Employee.DateOfBirth = employeeDto.DateOfBirth ?? user.Employee.DateOfBirth;
                    user.Employee.Address = employeeDto.Address ?? user.Employee.Address;

                    if (employeeDto.DepartmentId > 0)
                        user.Employee.DepartmentId = employeeDto.DepartmentId;

                    user.UpdatedAt = DateTime.UtcNow;
                    user.UpdatedBy = updatedByUserId;
                }

                var result = await _employeeRepo.SaveChangesAsync();
                if (!result)
                    throw new Exception("Failed to update employee.");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the employee: {ex.Message}");
            }
        }
        public async Task<List<EmployeeResponseDTO>> GetAllEmployeesAsync()
        {
            try
            {
                var employees = await _userRepo.GetAllEmployeesAsync();
                return employees ?? new List<EmployeeResponseDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching employees.", ex);
            }
        }
        public async Task<bool> ToggleEmployeeActivationAsync(int employeeId, bool isActive, int? updatedByUserId)
        {
            try
            {
                if (employeeId <= 0)
                    throw new ArgumentException("Invalid Employee Id.");

                var employee = await _employeeRepo.GetEmployeeByIdAsync(employeeId);
                if (employee == null)
                    throw new Exception($"Employee with id {employeeId} not found.");
                if (employee.IsActive == isActive)
                    throw new InvalidOperationException($"Employee is already {(isActive ? "active" : "inactive")}.");
                employee.IsActive = isActive;
                employee.UpdatedAt = DateTime.UtcNow;
                employee.UpdatedBy = updatedByUserId;

                var isSaved = await _employeeRepo.SaveChangesAsync();
                if (!isSaved)
                    throw new Exception($"Failed to {(isActive ? "activate" : "deactivate")} the employee.");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating employee status: {ex.Message}");
            }
        }
        public async Task<UserProfileDTO> GetLoggedInUserProfileAsync(int userId, int roleId)
        {
            try
            {
                if (userId <= 0)
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
                        DateOfBirth = (DateOnly)user.Employee.DateOfBirth,
                        Address = user.Employee.Address,
                        Leavebalance=user.Employee.LeaveBalance
                    };
                }
                return dto;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving employee details with timesheets: {ex.Message}", ex);
            }
        }
        public async Task<EmployeeDetailsWithTimesheetsDto?> GetEmployeeDetailsWithTimesheetsAsync(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    throw new ArgumentException("Invalid Employee ID.");

                var employee = await _employeeRepo.GetEmployeeWithDetailsAsync(employeeId);
                if (employee == null)
                    throw new Exception($"Employee with ID {employeeId} not found.");

                var timesheets = await _employeeRepo.GetTimesheetsByEmployeeIdAsync(employeeId);

                return new EmployeeDetailsWithTimesheetsDto
                {
                    EmployeeId = employee.EmployeeId,
                    Name = employee.User.FirstName,
                    Email = employee.User.Email,
                    Phone = employee.User.Phone,
                    Department = employee.Department?.DepartmentName ?? "N/A",
                    Timesheets = timesheets.Select(t => new TimesheetDto
                    {
                        TimesheetId = t.TimesheetId,
                        Date = t.Date,
                        HoursWorked = (double)t.TotalHours
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving employee details with timesheets: {ex.Message}", ex);
            }
        }


        public async Task<byte[]> ExportTimesheetsToExcelAsync(int employeeId)
        {
            try
            {
                var employee = await _employeeRepo.GetEmployeeWithDetailsAsync(employeeId);
                if (employee == null) return null;

                var timesheets = await _timesheetRepository.GetTimesheetsByEmployeeIdAsync(employeeId);

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Timesheets");

                // Headers
                worksheet.Cell(1, 1).Value = "Employee ID";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Email";
                worksheet.Cell(1, 4).Value = "Department";
                worksheet.Cell(1, 5).Value = "Timesheet ID";
                worksheet.Cell(1, 6).Value = "Date";
                worksheet.Cell(1, 6).Value = "Start Time";
                worksheet.Cell(1, 6).Value = "End Time";
                worksheet.Cell(1, 9).Value = "Hours Worked";

                var row = 2;
                foreach (var timesheet in timesheets)
                {
                    worksheet.Cell(row, 1).Value = employee.EmployeeId;
                    worksheet.Cell(row, 2).Value = $"{employee.User.FirstName} {employee.User.LastName}";
                    worksheet.Cell(row, 3).Value = employee.User.Email;
                    worksheet.Cell(row, 4).Value = employee.Department.DepartmentName;
                    worksheet.Cell(row, 5).Value = timesheet.TimesheetId;
                    worksheet.Cell(row, 6).Value = timesheet.Date.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 7).Value = timesheet.StartTime.ToString();
                    worksheet.Cell(row, 8).Value = timesheet.EndTime.ToString();
                    worksheet.Cell(row, 9).Value = timesheet.TotalHours;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while exporting timesheets: {ex.Message}");
            }
        }

        public async Task<bool> UpdateEmployeeProfileAsync(int? userId, UpdateEmployeeProfileDto dto)
        {
            try
            {
                if (userId == null || userId <= 0)
                    throw new ArgumentException("Invalid User ID.");

                var employee = await _employeeRepo.GetEmployeeByUserIdAsync(userId);
                if (employee == null)
                    throw new Exception("Employee not found.");

                // Updating fields
                employee.User.Phone = dto.Phone;
                employee.TechStack = dto.TechStack;
                employee.Address = dto.Address;
                employee.User.UpdatedAt = DateTime.UtcNow;

                var isUpdated = await _employeeRepo.UpdateEmployeeAsync(employee);
                if (!isUpdated)
                    throw new Exception("Failed to update employee profile.");

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the employee profile: {ex.Message}");
            }
        }
    }
}