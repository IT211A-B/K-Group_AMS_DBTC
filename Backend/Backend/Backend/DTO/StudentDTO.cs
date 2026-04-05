namespace Backend.Backend.DTOs
{
    public class GetStudentDTO
    {
        public string Student_ID { get; set; } = null!;
        public int Program_ID { get; set; }
        public int Department_ID { get; set; }
        public required int Year_Level { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }

    public class AddStudentDTO
    {
        public int Program_ID { get; set; }
        public required int Department_ID { get; set; }
        public required int Year_Level { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}