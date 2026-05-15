namespace Backend.Backend.DTOs
{
    public class GetPermissionDTO
    {
        public int Permission_ID { get; set; }
        public required string String_Name { get; set; }
    }

    public class AddPermissionDTO
    {
        public required string String_Name { get; set; }

    }
}