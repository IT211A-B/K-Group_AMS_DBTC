namespace Backend.Backend.DTOs
{
    public class GetCourseDTO
    {
        public int Course_ID { get; set; }
        public required string Title { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public required int Teacher_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }

    public class AddCourseDTO
    {
        public required string Title { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public required int Teacher_ID { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}