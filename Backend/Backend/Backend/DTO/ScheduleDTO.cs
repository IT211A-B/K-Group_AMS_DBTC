using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Backend.DTOs
{
    public class GetScheduleDTO
    {
        public int Schedule_Id { get; set; }
        public required int Course_ID { get; set; }
        public required int Course_Year { get; set; }
        public required int Department_ID { get; set; }
        public required int Program_ID { get; set; }
        public required DayOfWeek DayOfWeek { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
    }

    public class AddScheduleDTO
    {
        public required int Course_ID { get; set; }
        public required int Course_Year { get; set; }
        public required int Department_ID { get; set; }
        public required int Program_ID { get; set; }
        public required DayOfWeek DayOfWeek { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
    }
}