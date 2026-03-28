using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class Permission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Permission_ID { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Permission_Description { get; set; }

        [Required]
        public int Access_ID { get; set; }
    }
}