using AttStatus = Backend.Backend.Helper.Enum.AttendanceEnum.AttStatus;

namespace Backend.Backend.DTOs
{
    public class GetAttendanceDTO
    {
        public int Attendance_ID { get; set; }
        public int Enrollment_ID { get; set; }
        public DateTime Date { get; set; }
        public required AttStatus Status { get; set; }
    }

    public class AddAttendanceDTO
    {
        public required int Enrollment_ID { get; set; }
        public DateTime Date { get; set; }
        public required AttStatus Status { get; set; }
    }
}