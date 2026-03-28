using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Student_ID { get; set; }

        [Required]
        public int Program_ID { get; set; }

        [Required]
        public required int Department_ID { get; set; }

        [Required]
        public required int Year_Level { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        [MaxLength(210)]
        public string? CreatedBy { get; set; }

        [MaxLength(210)]
        public string? LastUpdatedBy { get; set; }
    }
}