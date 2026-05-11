namespace Frontend.Models
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsPresent { get; set; }
    }
}