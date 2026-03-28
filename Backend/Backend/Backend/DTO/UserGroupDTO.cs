namespace Backend.Backend.DTOs
{
    public class GetUserGroupDTO
    {
        public int Group_ID { get; set; }
        public required string Group_Name { get; set; }
        public string? Group_Description { get; set; }
        public DateTime Group_Created { get; set; }
        public required int Role_ID { get; set; }
        public required int Permission_ID { get; set; }
    }

    public class AddUserGroupDTO
    {
        public required string Group_Name { get; set; }
        public string? Group_Description { get; set; }
        public required int Role_ID { get; set; }
        public required int Permission_ID { get; set; }
    }
}