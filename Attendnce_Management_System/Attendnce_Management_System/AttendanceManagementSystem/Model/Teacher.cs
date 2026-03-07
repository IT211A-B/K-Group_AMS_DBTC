using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendnce_Management_System.AttendanceManagementSystem.Model
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Teacher_id { get; set; }

        [Required]
        public required string Full_Name { get; set; }

        public string? Email { get; set; }

        [Required]
        public required string Phone_Number { get; set; }

        [Required]
        public required char Gender { get; set; }

        [Required]
        public required DateTime Birth_Date { get; set; }
        public string? Address { get; set; }
        [Required]
        public required string Department { get; set; }
        [Required]
        public string Position { get; set; } = "Teacher";
        public DateTime CreatedAt {get; set;}
        public DateTime LastUpdatedAt {get; set;}
        public string? CreatedBy {get; set; }
        public string? LastUpdatedBy {get; set; }
    }
}
