using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Security.Claims;

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
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GetAll()
        {
            try { 
                var students = await _studentService.GetAllAsync();
                if (!(students.Data?.Any() ?? false))
                    return NotFound("No students found.");

                return Ok(students);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            try { 
                var student = await _studentService.GetByIdAsync(id);
                if (student == null)
                    return NotFound($"Student with ID {id} not found.");

                return Ok(student);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpGet("{id}/qr")]
        public async Task<IActionResult> GetQr(int id)
        {
            var student = await _studentService.getQrById(id); 

            if (student == null)
                return NotFound();

            return File(student, "image/png");
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Add(AddStudentDTO dto)
        {
            try {
                string? uuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(uuid))
                    throw new Exception("No Operator has been found");

                var student = await _studentService.AddAsync(dto, uuid);
                // Throw Error if Program Does not Exist or Name Sense is Bad, Only for last chance error or last defense if bug exist
                if (student.Status_code == 503)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, "Service Unavailable: Program Does not Exist or The Program does not have");
                }

                if (student.Status_code == 409)
                    return Conflict($"Status Code 409 - Conflict: User is Already Taken, Please Double Check if The Inputted User is Correct");

                return Ok(student);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, AddStudentDTO dto)
        {
            try {
                string? uuid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(uuid))
                    throw new Exception("No Operator has been found");

                var student = await _studentService.UpdateAsync(id, dto, uuid);
                if (student == null)
                    return NotFound($"Student with ID {id} not found.");

                return Ok(student);
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
                var success = await _studentService.DeleteAsync(id);
                if (!success)
                    return NotFound($"Student with ID {id} not found.");

                return Ok($"Student with ID {id} deleted successfully.");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}