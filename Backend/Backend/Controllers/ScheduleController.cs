using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            try { 
                var schedules = await _scheduleService.GetAllAsync();
                if (!(schedules.Data?.Any() ?? false))
                    return NotFound("No schedules found");

                return Ok(schedules);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpGet("{DayOfWeek}/Get_Students_CurrentDay_Schedule")]
        [Authorize(Roles = "Admin,Student,Teacher")]
        public async Task<IActionResult> GetScheduleFromAStudent(string DayOfWeek)
        {
            try
            {
                string? uuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(uuid))
                    throw new Exception("No Operator has been found");

                var schedules = await _scheduleService.GetCurrentStudentAttendance(uuid, DayOfWeek);
                Console.WriteLine("6");

                if (schedules is null)
                    return NotFound("No schedules found");
                Console.WriteLine("7");

                return Ok(schedules);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [Authorize(Roles = "Admin,Student,Teacher")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try { 
                var schedule = await _scheduleService.GetByIdAsync(id);
                if (schedule == null)
                    return NotFound($"#404! Schedule with ID {id} not found");

                return Ok(schedule);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(AddScheduleDTO schedule)
        {
            try { 
                var added = await _scheduleService.AddAsync(schedule);
                if (added.Status_code == 404)
                    return NotFound("404!: Course ID not Found");
                return Ok(added);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AddScheduleDTO schedule)
        {
            try { 
                var updated = await _scheduleService.UpdateAsync(id, schedule);
                if (updated == null)
                    return NotFound($"#404! Schedule with ID {id} not found");

                return Ok(updated);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try { 
                var deleted = await _scheduleService.DeleteAsync(id);
                if (!deleted)
                    return NotFound($"#404! Schedule with ID {id} not found");

                return Ok($"Schedule with ID {id} deleted successfully");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}