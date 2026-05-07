using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Backend.Backend.Model
{
    public class Schedule
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Schedule_Id { get; set; }

        [Required]
        public required int Course_ID { get; set; }
        public Course Course { get; set; } = null!;


        [Required]
        public required int Section_ID {  get; set; }
        public Section Section { get; set; } = null!;


        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        [Required]
        public required DayOfWeek DayOfWeek { get; set; }

        [Required]
        public required int Semester { get; set; }

        [Required]
        public required string AcademicYear { get; set; }

        [Required]
        public required TimeOnly StartTime { get; set; }

        [Required]
        public required TimeOnly EndTime { get; set; }
    }
}
