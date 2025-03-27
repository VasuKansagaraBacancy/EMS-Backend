using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Application.DTOs.ReportDTO;

namespace EMS.EMS.Domain.Interfaces
{
    public interface IReportsRepository
    {
        Task<List<EmployeeWorkHoursReportDto>> GetEmployeeWorkHoursReportAsync(ReportRequestDto request);
    }
}