namespace Backend.Backend.DTOs
{
    public class GetCourseDTO
    {
        public int Course_ID { get; set; }
        public required string Title { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public int? Section_ID { get; set; }
        public string? Section_Code { get; set; }
    }

    public class AddCourseDTO
    {
        public required string Title { get; set; }
        public required string Code { get; set; }
        public string? Description { get; set; }
        public required int Section_ID { get; set; }
    }
}