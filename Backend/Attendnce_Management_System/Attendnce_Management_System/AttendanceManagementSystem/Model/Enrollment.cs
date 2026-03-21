using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_Management_System.AttendanceManagementSystem.Model
{
    public class Enrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Enrollment_ID { get; set; }

        [Required]
        public required int Student_ID { get; set; }

        [Required]
        public required int Schedule_ID { get; set; }
    }
}
