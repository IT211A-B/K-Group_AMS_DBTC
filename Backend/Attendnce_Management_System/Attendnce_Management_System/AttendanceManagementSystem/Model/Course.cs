using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_Management_System.AttendanceManagementSystem.Model
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Course_ID { get; set; }

        [Required]
        public required string Title { get; set; }

        [Required]
        public required string Code { get; set; }

        public string? Description { get; set; }

        [Required]
        public required int Teacher_ID { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? LastUpdatedBy { get; set; }
    }
}