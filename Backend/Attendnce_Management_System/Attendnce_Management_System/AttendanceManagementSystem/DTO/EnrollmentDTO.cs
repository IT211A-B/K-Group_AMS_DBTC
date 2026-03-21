namespace Attendance_Management_System.AttendanceManagementSystem.DTOs
{
    public class GetEnrollmentDTO
    {
        public int Enrollment_ID { get; set; }
        public required int Student_ID { get; set; }
        public required int Schedule_ID { get; set; }
    }

    public class AddEnrollmentDTO
    {
        public required int Student_ID { get; set; }
        public required int Schedule_ID { get; set; }
    }
}