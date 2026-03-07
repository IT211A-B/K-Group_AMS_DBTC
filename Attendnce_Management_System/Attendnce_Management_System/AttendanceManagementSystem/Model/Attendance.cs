using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendnce_Management_System.AttendanceManagementSystem.Model
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Attendance_id { get; set; }

        [Required]
        public required int Student_id { get; set; }

        public Student? Student { get; set; }

        [Required]
        public required int Course_id { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public required string Status { get; set; } // Present, Absent, Late

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? LastUpdatedBy { get; set; }
    }
}