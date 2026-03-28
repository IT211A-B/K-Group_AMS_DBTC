using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Status = Backend.Backend.Helper.Enum.Status;

namespace Backend.Backend.Model
{
    public class Attendance
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Attendance_ID { get; set; }

        [Required]
        public required int Enrollment_ID { get; set; }

        [Required]
        public required DateTime Date { get; set; }

        [Required]
        public required Status Status { get; set; } // Present, Absent, Late, Excused

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        [MaxLength(50)]
        public string? LastUpdatedBy { get; set; }
    }
}