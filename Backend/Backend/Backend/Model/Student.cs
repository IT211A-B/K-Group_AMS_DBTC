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
        [Key]
        public string Student_ID { get; set; } = Ulid.NewUlid().ToString();

        [Required]
        [MaxLength(20)]
        public required string DocumentSeries { get; set; }

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