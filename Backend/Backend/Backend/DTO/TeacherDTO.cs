using System.ComponentModel.DataAnnotations;

namespace Backend.Backend.DTOs
{
    public class GetTeacherDTO
    {
        public string? DocumentSeries { get; set; }
        public required string UserDocumentSeries { get; set; }
        public required int DepartmentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }

    public class AddTeacherDTO
    {
        public required int User_ID { get; set; }
        public required int DepartmentId { get; set; }
    }
}