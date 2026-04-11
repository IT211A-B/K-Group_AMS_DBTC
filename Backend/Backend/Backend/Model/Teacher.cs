using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    /*
    Teacher Table
    - uses GUID / UUID (ULID) for 
        - security purposes
        - privacy for the teacher
        - (v7 like) for sorting using epoch timestamp and random data
        - new record cant be late with the old record (better for sorting)
        - Avoids collisions if not generated exact time, milisec accuracy
    */
    public class Teacher
    {
        [Key]
        public string Teacher_ID { get; set; } = Ulid.NewUlid().ToString();

        [MaxLength(16)]
        public required string DocumentSeries { get; set; }

        [MaxLength(50)]
        [Required]
        public required string Department { get; set; }

        public DateTime CreatedAt {get; set;}

        public DateTime LastUpdatedAt {get; set;}

        [MaxLength(50)]
        public string? CreatedBy {get; set; }

        [MaxLength(50)]
        public string? LastUpdatedBy {get; set; }
    }
}
