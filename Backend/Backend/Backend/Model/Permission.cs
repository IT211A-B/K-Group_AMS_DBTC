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
        [MaxLength(20)]
        public required string String_Name { get; set; }
    }
}