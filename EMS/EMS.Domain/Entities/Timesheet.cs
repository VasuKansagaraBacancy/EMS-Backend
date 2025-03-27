using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.EMS.Domain.Entities
{
    public class Timesheet : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TimesheetId { get; set; }

        [Required(ErrorMessage = "EmployeeId is required.")]
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [Column(TypeName = "date")]
        public DateOnly Date { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public TimeOnly EndTime { get; set; }

        [Required(ErrorMessage = "Total hours are required.")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal TotalHours { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            if (StartTime >= EndTime)
            {
                validationResults.Add(new ValidationResult(
                    "Start time must be earlier than End time.",
                    new[] { nameof(StartTime), nameof(EndTime) }
                ));
            }
            return validationResults;
        }
    }
}
