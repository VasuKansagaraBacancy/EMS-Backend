using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Domain.Entities
{
    public class Leave
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LeaveId { get; set; }

        [Required(ErrorMessage = "EmployeeId is required.")]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        public DateOnly EndDate { get; set; }
        public required string LeaveType { get; set; }
        public string? Reason { get; set; }
        public required string Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
