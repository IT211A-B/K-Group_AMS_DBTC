namespace Backend.Backend.DTOs
{
    public class GetSectionDTO
    {
        public int Section_Id { get; set; }
        public string? Section_Code { get; set; }
        public int Course_ID { get; set; }
        public string? Course_Code { get; set; }
        public string? Course_Title { get; set; }
    }

    public class AddSectionDTO
    {
        public required string Section_Code { get; set; }
        public required int Course_ID { get; set; }
    }
}