using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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
    public class User : IdentityUser
    {
        [Required]
        [MaxLength(16)]
        public required string DocumentSeries { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Full_Name { get; set; }

        [MaxLength(13)]
        public string? Phone_Number { get; set; }

        [MaxLength(1)]
        public char? Sex { get; set; }

        public DateTime? Birth_Date { get; set; }

        [MaxLength(512)]
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        [MaxLength(210)]
        public string? CreatedBy { get; set; }

		[MaxLength(210)]
		public string? LastUpdatedBy { get; set; }
    }
}
