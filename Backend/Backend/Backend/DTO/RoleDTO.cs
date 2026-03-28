namespace Backend.Backend.DTOs
{
    public class GetRoleDTO
    {
        public int Role_ID { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class AddRoleDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}