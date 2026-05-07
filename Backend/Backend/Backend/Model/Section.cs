using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class Section
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Section_Id { get; set; }

        [Required]
        public required string Section_Code { get; set; }

        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
