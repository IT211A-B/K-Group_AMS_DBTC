namespace Backend.Backend.DTOs
{
    public class GetDepartmentDTO
    {
        public int Department_Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class AddDepartmentDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
}