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

        public int Schedule_ID { get; set; }
        public Schedule Schedule { get; set; } = null!; 

        public ICollection<AttendanceStudent> AttendanceStudents { get; set; }  = new List<AttendanceStudent>();

        [Required]
        public required AttStatus TeacherStatus { get; set; }  

        [Required]
        public required DateOnly Date {  get; set; }

        public DateTime CreatedAt { get; set; } 

        [MaxLength(50)]
        public string? CreatedBy { get; set; }
    }
}