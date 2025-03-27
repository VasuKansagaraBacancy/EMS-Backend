using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Application.DTOs.TimeSheetDTO
{
    public class TimesheetUpdateRequestDto
    {
        public int TimesheetId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [Column(TypeName = "date")]
        public DateOnly Date { get; set; }
        [Required(ErrorMessage = "Start time is required.")]
        public TimeOnly StartTime { get; set; }
        [Required(ErrorMessage = "End time is required.")]
        public TimeOnly EndTime { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}