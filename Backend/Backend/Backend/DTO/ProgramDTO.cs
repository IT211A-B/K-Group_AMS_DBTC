namespace Backend.Backend.DTOs
{
    public class GetProgramDTO
    {
        public int Program_Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class AddProgramDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}