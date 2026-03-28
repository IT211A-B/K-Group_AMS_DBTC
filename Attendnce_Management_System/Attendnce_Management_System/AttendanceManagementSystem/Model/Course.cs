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
        [MaxLength(50)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(6)]
        public required string Code { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }

        [Required]
        public required int Teacher_ID { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        [MaxLength(210)]
        public string? CreatedBy { get; set; }

        [MaxLength(210)]
        public string? LastUpdatedBy { get; set; }
    }
}