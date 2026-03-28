using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class Access
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Access_ID { get; set; }

        [Required]
        [MaxLength(512)]
        public required string Name { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }
    }
}