using EMS.EMS.Application.DTOs.TimeSheetDTO;
using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Entities;
using EMS.EMS.Domain.Interfaces;

namespace EMS.EMS.Application.Services
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _repository;
        private readonly ILogger<TimesheetService> _logger;
        public TimesheetService(ITimesheetRepository repository, ILogger<TimesheetService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<IEnumerable<TimesheetResponseDto>> GetTimesheetsForEmployeeAsync(int? userId)
        {
            try
            {
                var employee = await _repository.GetEmployeeByUserIdAsync(userId);
                if (employee == null)
                    throw new InvalidOperationException("Employee not found.");

                var timesheets = await _repository.GetTimesheetsByEmployeeIdAsync(employee.EmployeeId);

                return timesheets.Select(t => new TimesheetResponseDto
                {
                    TimesheetId = t.TimesheetId,
                    EmployeeId = t.EmployeeId,
                    Date = t.Date,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    TotalHours = t.TotalHours,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<TimesheetResponseDto?> GetTimesheetByIdAsync(int timesheetId, int? userId)
        {
            try
            {
                var employee = await _repository.GetEmployeeByUserIdAsync(userId);
                if (employee == null)
                    throw new InvalidOperationException("Employee not found.");

                var existing = await _repository.GetTimesheetByIdAsync(timesheetId);
                if (existing == null)
                    throw new InvalidOperationException("Timesheet not found.");

                if (existing.EmployeeId != employee.EmployeeId)
                    throw new UnauthorizedAccessException("You are not authorized to access this timesheet.");

                return new TimesheetResponseDto
                {
                    TimesheetId = existing.TimesheetId,
                    EmployeeId = existing.EmployeeId,
                    Date = existing.Date,
                    StartTime = existing.StartTime,
                    EndTime = existing.EndTime,
                    TotalHours = existing.TotalHours,
                    Description = existing.Description,
                    CreatedAt = existing.CreatedAt
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> AddTimesheetAsync(TimesheetRequestDto dto, int? userId)
        {
            try
            {
                var employee = await _repository.GetEmployeeByUserIdAsync(userId);
                if (employee == null)
                    throw new InvalidOperationException("Employee not found.");

                ValidateTimesheet(dto.StartTime, dto.EndTime);

                var existingTimesheet = await _repository.GetTimesheetByEmployeeIdAndDateAsync(employee.EmployeeId, dto.Date);
                if (existingTimesheet != null)
                    throw new InvalidOperationException($"Timesheet for date {dto.Date:yyyy-MM-dd} already exists.");

                var timesheet = new Timesheet
                {
                    EmployeeId = employee.EmployeeId,
                    Date = dto.Date,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    Description = dto.Description,
                    TotalHours = CalculateTotalHours(dto.StartTime, dto.EndTime),
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddTimesheetAsync(timesheet);
                return await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<bool> UpdateTimesheetAsync(TimesheetUpdateRequestDto dto, int? userId)
        {
            try
            {
                var employee = await _repository.GetEmployeeByUserIdAsync(userId);
                if (employee == null)
                    throw new InvalidOperationException("Employee not found.");

                var existing = await _repository.GetTimesheetByIdAsync(dto.TimesheetId);
                if (existing == null)
                    throw new InvalidOperationException("Timesheet not found.");

                if (existing.EmployeeId != employee.EmployeeId)
                    throw new UnauthorizedAccessException("You are not authorized to access this timesheet.");

                ValidateTimesheet(dto.StartTime, dto.EndTime);

                existing.Date = dto.Date;
                existing.StartTime = dto.StartTime;
                existing.EndTime = dto.EndTime;
                existing.Description = dto.Description;
                existing.TotalHours = CalculateTotalHours(dto.StartTime, dto.EndTime);

                await _repository.UpdateTimesheetAsync(existing);
                return await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region Private Helpers

        private void ValidateTimesheet(TimeOnly start, TimeOnly end)
        {
            var startLimit = new TimeOnly(8, 0);
            var endLimit = new TimeOnly(21, 0);

            if (start < startLimit || end > endLimit)
                throw new InvalidOperationException("Timesheet must be within working hours: 8 AM to 9 PM.");

            if (start >= end)
                throw new InvalidOperationException("Start time must be earlier than end time.");
        }

        private decimal CalculateTotalHours(TimeOnly start, TimeOnly end)
        {
            var duration = end.ToTimeSpan() - start.ToTimeSpan();
            var hours = (decimal)duration.TotalHours;

            if (hours <= 0)
                throw new InvalidOperationException("Total hours calculated is invalid.");

            return Math.Round(hours, 2);
        }

        #endregion
    }
}
