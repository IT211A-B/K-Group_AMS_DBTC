namespace Backend.Backend.DTOs
{
    public class DashboardStatsDTO
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int AbsencesToday { get; set; }
        public int PresentToday { get; set; }
        public int LateToday { get; set; }
    }

    public class GetAccountActivityDTO
    {
        public int Activity_Id { get; set; }
        public string? User_Id { get; set; }
        public required string Activity_Type { get; set; }
        public required string Description { get; set; }
        public string? Actor_Name { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetTeacherScheduleDTO
    {
        public int Schedule_Id { get; set; }
        public int Course_ID { get; set; }
        public string Course_Code { get; set; } = "";
        public string Course_Title { get; set; } = "";
        public int Section_ID { get; set; }
        public string Section_Code { get; set; } = "";
        public DayOfWeek DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public bool IsActiveNow { get; set; }
    }

    public class GetSessionStudentDTO
    {
        public string Student_Id { get; set; } = "";
        public string Student_Name { get; set; } = "";
        public string DocumentSeries { get; set; } = "";
        public string? AttendanceStatus { get; set; }
    }
}
