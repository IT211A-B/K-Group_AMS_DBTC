namespace Backend.Backend.DTOs
{
    public class GetPermissionDTO
    {
        public int Permission_ID { get; set; }
        public required string Permission_Description { get; set; }
        public required int Access_ID { get; set; }
    }

    public class AddPermissionDTO
    {
        public required string Permission_Description { get; set; }
        public required int Access_ID { get; set; }
    }
}