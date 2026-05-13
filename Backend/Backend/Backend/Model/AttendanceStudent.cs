using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AttStatus = Backend.Backend.Helper.Enum.AttendanceEnum.AttStatus;

namespace Backend.Backend.Model
{
    public class AttendanceStudent
    {
        [Required]
        public required int Attendance_Id { get; set; }
        public Attendance Attendance { get; set; } = null!;

        [Required]
        public required string Student_Id { get; set; }
        public Student Student { get; set; } = null!;

        [Required]
        public required AttStatus StudentAttendance { get; set; } = AttStatus.Unassigned;
    }
}
