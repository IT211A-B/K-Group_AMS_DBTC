using Microsoft.AspNetCore.Mvc;
using Attendance_Management_System.AttendanceManagementSystem.DTOs;
using Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Controllers
{
    /// <summary>
    /// Handles all operations related to Access management.
    /// </summary>
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IAccessService _accessService;

        public AccessController(IAccessService accessService)
        {
            _accessService = accessService;
        }

        /// <summary>
        /// Retrieve all access records
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches all access records stored in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description>No input parameters required</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns all access records</description></item>
        ///     <item><description>Returns 404 if no records exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Access
        /// </code>
        /// </remarks>
        /// <returns>List of access records</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAccesses()
        {
            var accesses = await _accessService.GetAllAsync();
            if (!accesses.Any())
                return NotFound("No Access Records Found");

            return Ok(accesses);
        }

        /// <summary>
        /// Retrieve an access record by ID
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches a specific access record using its unique identifier.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Unique ID of the access record</description></item>
        /// </list>  
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns the access record if found</description></item>
        ///     <item><description>Returns 404 if the record does not exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/Access/1
        /// </code>
        /// </remarks>
        /// <param name="id">Access ID</param>
        /// <returns>Access record</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAccessById(int id)
        {
            var access = await _accessService.GetByIdAsync(id);
            if (access == null)
                return NotFound($"#404! Access with ID {id} not found");

            return Ok(access);
        }

        /// <summary>
        /// Create a new access record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Creates a new access entry in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>Name</b> - Name of the access</description></item>
        ///     <item><description><b>Description</b> - Optional description</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Creates a new access record</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// POST /AttendanceManagement/Access
        /// {
        ///   "name": "Read",
        ///   "description": "Read-only access"
        /// }
        /// </code>
        /// </remarks>
        /// <param name="dto">Access data</param>
        /// <returns>Created access record</returns>
        [HttpPost]
        public async Task<IActionResult> AddAccess(AddAccessDTO dto)
        {
            var access = await _accessService.AddAsync(dto);
            return Ok(access);
        }

        /// <summary>
        /// Update an existing access record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Updates an existing access record using its ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Access ID</description></item>
        ///     <item><description><b>Name</b> - Updated name</description></item>
        ///     <item><description><b>Description</b> - Updated description</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Updates the record if it exists</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Access ID</param>
        /// <param name="dto">Updated access data</param>
        /// <returns>Updated access record</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAccess(int id, AddAccessDTO dto)
        {
            var updatedAccess = await _accessService.UpdateAsync(id, dto);
            if (updatedAccess == null)
                return NotFound($"#404! Access with ID {id} not found");

            return Ok(updatedAccess);
        }

        /// <summary>
        /// Delete an access record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Deletes an access record from the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Access ID to delete</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Deletes the record if found</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">Access ID</param>
        /// <returns>Status message</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAccess(int id)
        {
            var success = await _accessService.DeleteAsync(id);
            if (!success)
                return NotFound($"#404! Access with ID {id} not found");

            return Ok($"Access with ID {id} deleted successfully");
        }
    }
}