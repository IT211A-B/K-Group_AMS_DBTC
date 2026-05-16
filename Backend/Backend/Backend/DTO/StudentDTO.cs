using Backend.Backend.Model;
using System.ComponentModel.DataAnnotations;

namespace Backend.Backend.DTOs
{
    public class GetStudentDTO
    {
        public required string UserDocumentSeries{ get; set; }
        public required string DocumentSeries { get; set; }
        public int Program_ID { get; set; }
        public required int Department_ID { get; set; }
        public required int SectionID { get; set; }
        public required int Year_Level { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }

    public class AddStudentDTO
    {
        public int Program_ID { get; set; }
        public required int SectionID { get; set; }
        public required int Department_ID { get; set; }
        public required int Year_Level { get; set; }
    }

    public class GetStudentItsCourse
    {
        public int Course_ID { get; set; }
        public string? Title { get; set; }
        public string? Code { get; set; }
        public string? Full_Name { get; set; }
    }
}