using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_Management_System.AttendanceManagementSystem.Model
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Role_ID { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? Description { get; set; }
    }
}