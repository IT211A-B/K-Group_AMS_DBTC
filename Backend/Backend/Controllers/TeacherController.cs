using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin,Student")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(AddTeacherDTO dto)
        {
            try {
                string? uuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(uuid))
                    throw new Exception("No Operator has been found");

                var teacher = await _teacherService.AddAsync(dto, uuid);
                return Ok(teacher);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, AddTeacherDTO dto)
        {
            try {
                string? uuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(uuid))
                    throw new Exception("No Operator has been found");

                var teacher = await _teacherService.UpdateAsync(id, dto, uuid);
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
        [Authorize(Roles = "Admin")]
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