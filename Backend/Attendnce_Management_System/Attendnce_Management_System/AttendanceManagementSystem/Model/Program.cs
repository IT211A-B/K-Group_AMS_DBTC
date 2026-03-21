using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_Management_System.AttendanceManagementSystem.Model
{
    public class Program_
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Program_Id { get; set; }

        [Required]
        [MaxLength(255)]
        public required string Name { get; set; }

        [MaxLength(511)]
        public string? Description { get; set; }
    }
}