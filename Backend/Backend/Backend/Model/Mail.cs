using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class Mail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Mail_Id { get; set; }

        [Required]
        [MaxLength(36)]
        public required string Sender_User_Id { get; set; }

        [MaxLength(100)]
        public string? Sender_Name { get; set; }

        [MaxLength(20)]
        public string? Sender_Role { get; set; }

        /// <summary>User id, or broadcast: all, teacher, student</summary>
        [Required]
        [MaxLength(36)]
        public required string Recipient_Target { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Subject { get; set; }

        [Required]
        public required string Body { get; set; }

        [MaxLength(30)]
        public string Type { get; set; } = "message";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
