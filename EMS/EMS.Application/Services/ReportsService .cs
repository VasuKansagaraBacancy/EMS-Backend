using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Application.DTOs.ReportDTO;
using EMS.EMS.Application.Interfaces;
using EMS.EMS.Domain.Interfaces;

namespace EMS.EMS.Application.Services
{
    public class ReportsService : IReportsService
    {
        private readonly IReportsRepository _repository;
        public ReportsService(IReportsRepository repository)
        {
            _repository = repository;
        }
        public async Task<List<EmployeeWorkHoursReportDto>> GetEmployeeWorkHoursReportAsync(ReportRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Request data cannot be null.");
                }
                if (request.StartDate > request.EndDate)
                {
                    throw new ArgumentException("Start date cannot be greater than end date.");
                }
                if (string.IsNullOrEmpty(request.GroupBy))
                {
                    request.GroupBy = "weekly";
                }
                var reports = await _repository.GetEmployeeWorkHoursReportAsync(request);
                if (reports == null || !reports.Any())
                {
                    return new List<EmployeeWorkHoursReportDto>(); // Returning empty list instead of throwing exception
                }
                return reports;
            }
             catch (ArgumentNullException ex)
            {
                throw new ArgumentNullException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}