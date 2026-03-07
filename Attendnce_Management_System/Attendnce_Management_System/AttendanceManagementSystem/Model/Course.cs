using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendnce_Management_System.AttendanceManagementSystem.Model
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Course_id { get; set; }

        [Required]
        public required string Course_Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public required int Teacher_id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? LastUpdatedBy { get; set; }
    }
}