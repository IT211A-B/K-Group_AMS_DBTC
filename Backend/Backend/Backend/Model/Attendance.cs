using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AttStatus = Backend.Backend.Helper.Enum.AttendanceEnum.AttStatus;

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
        public required AttStatus Status { get; set; } 

        public DateTime CreatedAt { get; set; } 

        public DateTime LastUpdatedAt { get; set; }

        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        [MaxLength(50)]
        public string? LastUpdatedBy { get; set; }
    }
}