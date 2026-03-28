using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_Management_System.AttendanceManagementSystem.Model
{
    public class Access
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Access_ID { get; set; }

        [Required]
        [MaxLength(512)]
        public required string Name { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }
    }
}