using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Application.DTOs.ReportDTO;

namespace EMS.EMS.Application.Interfaces
{
    public interface IReportsService
    {
        Task<List<EmployeeWorkHoursReportDto>> GetEmployeeWorkHoursReportAsync(ReportRequestDto request);
    }
}