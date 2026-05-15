using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.Model
{
    public class Course
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Course_ID { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Title { get; set; }

        [Required]
        [MaxLength(8)]
        public required string Code { get; set; }

        [MaxLength(512)]
        public string? Description { get; set; }

        public string? Teacher_ID { get; set; }
        public Teacher? Teacher { get; set; }

        public ICollection<Section> Sections { get; set; } = new List<Section>();
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        [MaxLength(210)]
        public string? CreatedBy { get; set; }

        [MaxLength(210)]
        public string? LastUpdatedBy { get; set; }
    }
}