using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.EMS.Application.DTOs.TimeSheetDTO
{
    public class TimesheetRequestDto
    {
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