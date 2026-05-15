using Backend.Backend.Model;
using System.ComponentModel.DataAnnotations;
using AttStatus = Backend.Backend.Helper.Enum.AttendanceEnum.AttStatus;

namespace Backend.Backend.DTOs
{
    public class GetAttendanceDTO
    {
        public int Attendance_ID { get; set; }
        public int Schedule_ID { get; set; }

        public required AttStatus TeacherStatus { get; set; }

        public required DateOnly Date { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }
    }

    public class GetRecordAttendanceOfCertainStudent
    {
        public int Attendance_ID { get; set; }
        public string? Course_Title { get; set; }
        public string? Course_Code { get; set; }
        public DateOnly Date { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public AttStatus AttendanceStatus { get; set; }
    }
    public class GetRecordAttendanceOfCertainStudentServiceDTO
    {
        public int Attendance_ID { get; set; }
        public string? Course_Title { get; set; }
        public string? Course_Code { get; set; }
        public DateOnly Date { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public string? AttendanceStatus { get; set; }
    }
}