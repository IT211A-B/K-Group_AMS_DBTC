using Status = Attendance_Management_System.AttendanceManagementSystem.Helper.Enum.Status;

namespace Attendance_Management_System.AttendanceManagementSystem.DTOs
{
    public class GetAttendanceDTO
    {
        public int Attendance_ID { get; set; }
        public int Enrollment_ID { get; set; }
        public DateTime Date { get; set; }
        public required Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }

    public class AddAttendanceDTO
    {
        public required int Enrollment_ID { get; set; }
        public DateTime Date { get; set; }
        public required Status Status { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}