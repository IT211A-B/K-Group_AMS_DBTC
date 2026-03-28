using Microsoft.AspNetCore.Mvc;
using Attendance_Management_System.AttendanceManagementSystem.DTOs;
using Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Controllers
{
    /// <summary>
    /// Handles all operations related to programs.
    /// </summary>
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class ProgramController : ControllerBase
    {
        private readonly IProgramService _programService;

        public ProgramController(IProgramService programService)
        {
            _programService = programService;
        }

        /// <summary>
        /// Retrieves all program records.
        /// </summary>
        /// <remarks>
        /// Returns a list of all programs available in the system.
        /// </remarks>
        /// <returns>List of program records</returns>
        /// <response code="200">Programs retrieved successfully</response>
        /// <response code="404">No programs found</response>
        [HttpGet]
        public async Task<IActionResult> GetAllPrograms()
        {
            var programs = await _programService.GetAllAsync();
            if (!programs.Any())
                return NotFound("No Programs Found");

            return Ok(programs);
        }

        /// <summary>
        /// Retrieves a specific program by ID.
        /// </summary>
        /// <param name="id">The ID of the program</param>
        /// <returns>A single program record</returns>
        /// <response code="200">Program found</response>
        /// <response code="404">Program not found</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProgramById(int id)
        {
            var program = await _programService.GetByIdAsync(id);
            if (program == null)
                return NotFound($"#404! Program with ID {id} not found");

            return Ok(program);
        }

        /// <summary>
        /// Adds a new program.
        /// </summary>
        /// <param name="dto">Program data to create</param>
        /// <returns>The created program</returns>
        /// <response code="200">Program created successfully</response>
        [HttpPost]
        public async Task<IActionResult> AddProgram(AddProgramDTO dto)
        {
            var program = await _programService.AddAsync(dto);
            return Ok(program);
        }

        /// <summary>
        /// Updates an existing program.
        /// </summary>
        /// <param name="id">The ID of the program</param>
        /// <param name="dto">Updated program data</param>
        /// <returns>The updated program</returns>
        /// <response code="200">Program updated successfully</response>
        /// <response code="404">Program not found</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProgram(int id, AddProgramDTO dto)
        {
            var updatedProgram = await _programService.UpdateAsync(id, dto);
            if (updatedProgram == null)
                return NotFound($"#404! Program with ID {id} not found");

            return Ok(updatedProgram);
        }

        /// <summary>
        /// Deletes a program.
        /// </summary>
        /// <param name="id">The ID of the program</param>
        /// <returns>Confirmation message</returns>
        /// <response code="200">Program deleted successfully</response>
        /// <response code="404">Program not found</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            var success = await _programService.DeleteAsync(id);
            if (!success)
                return NotFound($"#404! Program with ID {id} not found");

            return Ok($"Program with ID {id} deleted successfully");
        }
    }
}