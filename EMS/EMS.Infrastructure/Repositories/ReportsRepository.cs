using EMS.EMS.Application.DTOs.EmployeeDTO;
using EMS.EMS.Application.DTOs.ReportDTO;
using EMS.EMS.Domain.Interfaces;
using EMS.EMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EMS.EMS.Infrastructure.Repositories
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly EMSDbContext _context;
        public ReportsRepository(EMSDbContext context)
        {
            _context = context;
        }
        public async Task<List<EmployeeWorkHoursReportDto>> GetEmployeeWorkHoursReportAsync(ReportRequestDto request)
        {
            try
            {
                var query = _context.Timesheets
                                .Include(t => t.Employee)
                                    .ThenInclude(e => e.User)
                                .Where(t => t.Date >= request.StartDate && t.Date <= request.EndDate);

                if (request.EmployeeId.HasValue)
                {
                    query = query.Where(t => t.EmployeeId == request.EmployeeId.Value);
                }

                var timesheets = await query.ToListAsync();

                if (!timesheets.Any())
                {
                    return new List<EmployeeWorkHoursReportDto>();
                }

                var groupedReports = request.GroupBy.ToLower() switch
                {
                    "monthly" => timesheets
                        .GroupBy(t => new { t.EmployeeId, Month = $"{t.Date:yyyy-MM}" })
                        .Select(g => new EmployeeWorkHoursReportDto
                        {
                            EmployeeId = g.Key.EmployeeId,
                            EmployeeName = g.First().Employee?.User != null
                                ? $"{g.First().Employee.User.FirstName} {g.First().Employee.User.LastName}"
                                : "Unknown",
                            Period = g.Key.Month,
                            TotalHours = (double)g.Sum(x => x.TotalHours)
                        })
                        .ToList(),

                    _ => timesheets
                        .GroupBy(t =>
                        {
                            var dateTime = t.Date.ToDateTime(TimeOnly.MinValue);
                            return new
                            {
                                t.EmployeeId,
                                Week = $"{ISOWeek.GetYear(dateTime)}-W{ISOWeek.GetWeekOfYear(dateTime)}"
                            };
                        })
                        .Select(g => new EmployeeWorkHoursReportDto
                        {
                            EmployeeId = g.Key.EmployeeId,
                            EmployeeName = g.First().Employee?.User != null
                                ? $"{g.First().Employee.User.FirstName} {g.First().Employee.User.LastName}"
                                : "Unknown",
                            Period = g.Key.Week,
                            TotalHours = (double)g.Sum(x => x.TotalHours)
                        })
                        .ToList()
                };
                return groupedReports;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to generate employee work hours report: {ex.Message}");
            }
        }
    }
}