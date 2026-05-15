namespace Backend.Backend.DTOs
{
    public class SendMailDTO
    {
        public required string RecipientId { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public string Type { get; set; } = "message";
    }

    public class GetMailDTO
    {
        public int Id { get; set; }
        public string RecipientId { get; set; } = "";
        public string SenderUserId { get; set; } = "";
        public string SenderName { get; set; } = "";
        public string SenderRole { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Type { get; set; } = "message";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
