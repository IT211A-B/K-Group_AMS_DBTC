using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    /*
    Student Table
    - uses GUID / UUID (ULID) for 
        - security purposes
        - privacy for the student
        - (v7 like) for sorting using epoch timestamp and random data
        - new record cant be late with the old record (better for sorting)
        - Avoids collisions if not generated exact time, milisec accuracy
    */
    public class Student
    {
        private int _yearlevel;

        [Key]
        public string Student_ID { get; set; } = Ulid.NewUlid().ToString();

        [Required]
        public required string User_ID { get; set; }
        public User User { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public required string DocumentSeries { get; set; }

        [Required]
        public int Program_ID { get; set; }
        public Program_ Program { get; set; } = null!;

        [Required]
        public required int Department_ID { get; set; }
        public Department Department { get; set; } = null!;

        [Required]
        public required int SectionID { get; set; } 
        public Section Section { get; set; } = null!;

        public ICollection<AttendanceStudent> AttendanceStudents { get; set; } = new List<AttendanceStudent>();

        [Required]
        public required int Year_Level {
            get => _yearlevel;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException(nameof(Year_Level),
                "Year level must be at least 1.");
                _yearlevel = value;
            }
        }
        [MaxLength(255)]
        public required string QrToken { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        [MaxLength(210)]
        public string? CreatedBy { get; set; }

        [MaxLength(210)]
        public string? LastUpdatedBy { get; set; }
    }
}