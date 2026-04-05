namespace Backend.Backend.DTOs
{
    public class GetTeacherDTO
    {
        public string Teacher_ID { get; set; } = null!;
        public required string Department { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }

    public class AddTeacherDTO
    {
        public required string Department { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}