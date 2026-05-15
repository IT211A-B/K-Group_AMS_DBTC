using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class AttendanceToken
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceTokenId { get; set; }

        public required string Token { get; set; } // unique token string

        public DateTime Expiration { get; set; } // expiry time

        public bool IsUsed { get; set; } // prevents reuse

        public int CreatedByUserId { get; set; } // teacher/admin who generated it
    }
}
