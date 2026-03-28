using Microsoft.AspNetCore.Mvc;
using Backend.Backend.Model;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controllers
{
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var schedules = await _scheduleService.GetAllAsync();
            if (!schedules.Any())
                return NotFound("No schedules found");

            return Ok(schedules);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var schedule = await _scheduleService.GetByIdAsync(id);
            if (schedule == null)
                return NotFound($"#404! Schedule with ID {id} not found");

            return Ok(schedule);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Schedule schedule)
        {
            var added = await _scheduleService.AddAsync(schedule);
            return Ok(added);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Schedule schedule)
        {
            var updated = await _scheduleService.UpdateAsync(id, schedule);
            if (updated == null)
                return NotFound($"#404! Schedule with ID {id} not found");

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _scheduleService.DeleteAsync(id);
            if (!deleted)
                return NotFound($"#404! Schedule with ID {id} not found");

            return Ok($"Schedule with ID {id} deleted successfully");
        }
    }
}