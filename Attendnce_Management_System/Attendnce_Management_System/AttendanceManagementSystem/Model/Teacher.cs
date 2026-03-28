using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_Management_System.AttendanceManagementSystem.Model
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Teacher_ID { get; set; }

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
