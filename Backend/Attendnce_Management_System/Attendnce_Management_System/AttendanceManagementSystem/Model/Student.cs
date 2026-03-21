using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_Management_System.AttendanceManagementSystem.Model
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Student_ID { get; set; }

        [Required]
        public int Program_ID { get; set; }

        [Required]
        public required int Department_ID { get; set; }

        [Required]
        public required string Year_Level { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? LastUpdatedBy { get; set; }
    }
}