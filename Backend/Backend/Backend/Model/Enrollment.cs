using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class Enrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Enrollment_ID { get; set; }

        [Required]
        public required int Student_ID { get; set; }

        [Required]
        public required int Schedule_ID { get; set; }
    }
}
