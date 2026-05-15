using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Notification_Id { get; set; }

        [Required]
        [MaxLength(36)]
        public required string User_Id { get; set; }

        public int? Mail_Id { get; set; }
        public Mail? Mail { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        [Required]
        public required string Message { get; set; }

        [MaxLength(30)]
        public string Type { get; set; } = "message";

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
