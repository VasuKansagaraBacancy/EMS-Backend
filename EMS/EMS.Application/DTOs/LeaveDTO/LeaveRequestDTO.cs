namespace EMS.EMS.Application.DTOs.LeaveDTO
{
    public class LeaveRequestDTO
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public string LeaveType { get; set; } = string.Empty;
        public string? Reason { get; set; }
    }
}