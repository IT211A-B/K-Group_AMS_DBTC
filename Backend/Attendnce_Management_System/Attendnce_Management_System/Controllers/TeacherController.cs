using Microsoft.AspNetCore.Mvc;
using Attendance_Management_System.AttendanceManagementSystem.DTOs;
using Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teachers = await _teacherService.GetAllAsync();
            if (!teachers.Any())
                return NotFound("No teachers found.");

            return Ok(teachers);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher == null)
                return NotFound($"Teacher with ID {id} not found.");

            return Ok(teacher);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTeacherDTO dto)
        {
            var teacher = await _teacherService.AddAsync(dto);
            return Ok(teacher);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AddTeacherDTO dto)
        {
            var teacher = await _teacherService.UpdateAsync(id, dto);
            if (teacher == null)
                return NotFound($"Teacher with ID {id} not found.");

            return Ok(teacher);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _teacherService.DeleteAsync(id);
            if (!success)
                return NotFound($"Teacher with ID {id} not found.");

            return Ok($"Teacher with ID {id} deleted successfully.");
        }
    }
}