using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class AccountActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Activity_Id { get; set; }

        [MaxLength(36)]
        public string? User_Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Activity_Type { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Description { get; set; }

        [MaxLength(36)]
        public string? Actor_User_Id { get; set; }

        [MaxLength(100)]
        public string? Actor_Name { get; set; }

        public int? Related_Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
