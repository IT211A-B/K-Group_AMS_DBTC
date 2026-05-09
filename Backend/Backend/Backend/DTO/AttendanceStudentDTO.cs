using AttendanceEnum = Backend.Backend.Helper.Enum.AttendanceEnum.AttStatus;

namespace Backend.Backend.DTOs
{
    public class GetAttendanceStudentDTO
    {
        public required int Attendance_Id { get; set; }
        public required string StudentDocumentSeries { get; set; }
        public required AttendanceEnum StudentAttendanceStatus { get; set; }
    }

    public class AddAttendanceStudentDTO
    {
        public required int Attendance_Id { get; set; }
        public required int Student_Id { get; set; }
    }
}