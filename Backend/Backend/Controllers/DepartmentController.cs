using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controllers
{
    /// <summary>
    /// Handles all operations related to Department management.
    /// </summary>
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        /// <summary>
        /// Retrieve all departments
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches all departments available in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description>No input parameters required</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns all department records</description></item>
        ///     <item><description>Returns 404 if no departments exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Department
        /// </code>
        /// </remarks>
        /// <returns>List of departments</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            try { 
                var departments = await _departmentService.GetAllAsync();
                if (!(departments?.Data?.Any() ?? false))
                    return NotFound("No Departments Found");

                return Ok(departments);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Retrieve a department by ID
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches a specific department using its unique identifier.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Department ID</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns the department if found</description></item>
        ///     <item><description>Returns 404 if the department does not exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Department/1
        /// </code>
        /// </remarks>
        /// <param name="id">Department ID</param>
        /// <returns>Department details</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            try { 
                var department = await _departmentService.GetByIdAsync(id);
                if (department == null)
                    return NotFound($"#404! Department with ID {id} not found");

                return Ok(department);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Create a new department
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Creates a new department record in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>Name</b> - Department name</description></item>
        ///     <item><description><b>Description</b> - Optional description</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Creates a new department record</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// POST /AttendanceManagement/Department
        /// {
        ///   "name": "Basic Education",
        ///   "description": "Range From Grade 1 to Grade 12 (Senior High)"
        /// }
        /// </code>
        /// </remarks>
        /// <param name="dto">Department data</param>
        /// <returns>Created department</returns>
        [HttpPost]
        public async Task<IActionResult> AddDepartment(AddDepartmentDTO dto)
        {
            try { 
                var department = await _departmentService.AddAsync(dto);
                return Ok(department);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Update an existing department
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Updates a department record using its ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Department ID</description></item>
        ///     <item><description><b>Name</b> - Updated department name</description></item>
        ///     <item><description><b>Description</b> - Updated description</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Updates the department if it exists</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Department ID</param>
        /// <param name="dto">Updated department data</param>
        /// <returns>Updated department</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDepartment(int id, AddDepartmentDTO dto)
        {
            try { 
                var updatedDepartment = await _departmentService.UpdateAsync(id, dto);
                if (updatedDepartment == null)
                    return NotFound($"#404! Department with ID {id} not found");

                return Ok(updatedDepartment);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Delete a department
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Deletes a department record from the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Department ID to delete</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Deletes the department if found</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Department ID</param>
        /// <returns>Status message</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try { 
                var success = await _departmentService.DeleteAsync(id);
                if (!success)
                    return NotFound($"#404! Department with ID {id} not found");

                return Ok($"Department with ID {id} deleted successfully");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}