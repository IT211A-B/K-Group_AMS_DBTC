using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllAsync();
            if (!students.Any())
                return NotFound("No students found.");

            return Ok(students);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null)
                return NotFound($"Student with ID {id} not found.");

            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddStudentDTO dto)
        {
            var student = await _studentService.AddAsync(dto);
            return Ok(student);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AddStudentDTO dto)
        {
            var student = await _studentService.UpdateAsync(id, dto);
            if (student == null)
                return NotFound($"Student with ID {id} not found.");

            return Ok(student);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _studentService.DeleteAsync(id);
            if (!success)
                return NotFound($"Student with ID {id} not found.");

            return Ok($"Student with ID {id} deleted successfully.");
        }
    }
}