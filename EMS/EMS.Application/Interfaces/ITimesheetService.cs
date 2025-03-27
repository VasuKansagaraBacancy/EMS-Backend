using EMS.EMS.Application.DTOs.TimeSheetDTO;

namespace EMS.EMS.Application.Interfaces
{
    public interface ITimesheetService
    {
        Task<IEnumerable<TimesheetResponseDto>> GetTimesheetsForEmployeeAsync(int? UserId);
        Task<TimesheetResponseDto?> GetTimesheetByIdAsync(int timesheetId, int? UserId);
        Task<bool> AddTimesheetAsync(TimesheetRequestDto dto, int? UserId);
        Task<bool> UpdateTimesheetAsync(TimesheetUpdateRequestDto dto, int? UserId);

    }
}