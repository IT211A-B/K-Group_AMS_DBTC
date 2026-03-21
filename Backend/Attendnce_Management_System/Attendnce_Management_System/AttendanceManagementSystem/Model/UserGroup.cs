using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Attendance_Management_System.AttendanceManagementSystem.Model
{
    public class UserGroup
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Group_ID { get; set; }   // Primary Key

        [Required]
        public required string Group_Name { get; set; }   // Name of the group

        public string? Group_Description { get; set; }   

        public DateTime Group_Created { get; set; }       

        [Required]
        public required int Role_ID { get; set; }   

        [Required]
        public required int Permission_ID { get; set; }   
    }
}