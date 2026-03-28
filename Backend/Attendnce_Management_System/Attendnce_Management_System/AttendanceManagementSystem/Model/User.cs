using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_Management_System.AttendanceManagementSystem.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int User_ID { get; set; }

        [Required]
        [StringLength(255)]
        public required string Full_Name { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string PassHash { get; set; }

        public string? Phone_Number { get; set; }

        public char? Gender { get; set; }

        public DateTime? Birth_Date { get; set; }
        public string? Address { get; set; }
        public int? UserGroup_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}
