namespace Backend.Backend.DTOs
{
    public class GetTeacherDTO
    {
        public int Teacher_ID { get; set; }
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