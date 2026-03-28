namespace Backend.Backend.DTOs
{
    public class GetAccessDTO
    {
        public int Access_ID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class AddAccessDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}