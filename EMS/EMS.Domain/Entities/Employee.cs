using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EMS.EMS.Domain.Entities
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "DepartmentId is required.")]
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string? TechStack { get; set; }

        public int LeaveBalance { get; set; } = 20;

        public ICollection<Timesheet>? Timesheets { get; set; }

    }
}
