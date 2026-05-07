namespace Backend.Backend.DTOs
{
    public class GetSectionDTO
    {
        public int Section_Id { get; set; }
        public string? Section_Code { get; set; }
    }

    public class AddSectionDTO
    {
        public required string Section_Code { get; set; }
    }
}