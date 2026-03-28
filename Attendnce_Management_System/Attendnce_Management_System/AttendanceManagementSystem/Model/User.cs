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
        [MaxLength(255)]
        public required string Full_Name { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public required string PassHash { get; set; }

        [MaxLength(13)]
        public string? Phone_Number { get; set; }

        [MaxLength(1)]
        public char? Gender { get; set; }

        public DateTime? Birth_Date { get; set; }

        [MaxLength(512)]
        public string? Address { get; set; }
        public int? UserGroup_ID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        [MaxLength(210)]
        public string? CreatedBy { get; set; }

		[MaxLength(210)]
		public string? LastUpdatedBy { get; set; }
    }
}
