using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controller
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
            try { 
                var teachers = await _teacherService.GetAllAsync();
                if (!(teachers.Data?.Any() ?? false))
                    return NotFound("No teachers found.");

                return Ok(teachers);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try { 
                var teacher = await _teacherService.GetByIdAsync(id);
                if (teacher == null)
                    return NotFound($"Teacher with ID {id} not found.");

                return Ok(teacher);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTeacherDTO dto)
        {
            try { 
                var teacher = await _teacherService.AddAsync(dto);
                return Ok(teacher);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AddTeacherDTO dto)
        {
            try { 
                var teacher = await _teacherService.UpdateAsync(id, dto);
                if (teacher == null)
                    return NotFound($"Teacher with ID {id} not found.");

                return Ok(teacher);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try { 
                var success = await _teacherService.DeleteAsync(id);
                if (!success)
                    return NotFound($"Teacher with ID {id} not found.");

                return Ok($"Teacher with ID {id} deleted successfully.");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}