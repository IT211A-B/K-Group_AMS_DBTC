using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Schedule_Id { get; set; }

        [Required]
        public required int Course_ID { get; set; }

        [Required]
        public required int Course_Year { get; set; }

        [Required]
        public required int Department_ID { get; set; }

        [Required]
        public required int Program_ID { get; set; }   

        [Required]
        public required DayOfWeek DayOfWeek { get; set; }

        [Required]
        public required TimeOnly StartTime { get; set; }

        [Required]
        public required TimeOnly EndTime { get; set; }
    }
}
