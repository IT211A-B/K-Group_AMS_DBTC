namespace Backend.Backend.DTOs
{
    public class GetTeacherDTO
    {
        public string? DocumentSeries { get; set; }
        public required int User_ID { get; set; }
        public required int DepartmentId { get; set; }
    }

    public class AddTeacherDTO
    {
        public required int User_ID { get; set; }
        public required int DepartmentId { get; set; }
    }
}