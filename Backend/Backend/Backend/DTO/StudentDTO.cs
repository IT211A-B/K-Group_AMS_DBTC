namespace Backend.Backend.DTOs
{
    public class GetStudentDTO
    {
        public required string DocumentSeries { get; set; }
        public required int User_ID { get; set; }
        public int Program_ID { get; set; }
        public int Department_ID { get; set; }
        public required int Year_Level { get; set; }
    }

    public class AddStudentDTO
    {
        public required int User_ID { get; set; }
        public int Program_ID { get; set; }
        public required int Department_ID { get; set; }
        public required int Year_Level { get; set; }
    }

    public class StudentResponse
    {
        public required int Status_Code { get; set; }
        public GetStudentDTO? data { get; set; }
    }
}