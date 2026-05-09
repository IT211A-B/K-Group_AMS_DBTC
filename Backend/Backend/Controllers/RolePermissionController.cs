using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controllers
{
    /// <summary>
    /// Handles all operations related to RolePermission management.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionService _rolepermissionService;

        public RolePermissionController(IRolePermissionService rolepermissionService)
        {
            _rolepermissionService = rolepermissionService;
        }

        /// <summary>
        /// Retrieve all Role Permissions records
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches all Role Permissions records stored in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description>No input parameters required</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns all Role Permissions records</description></item>
        ///     <item><description>Returns 404 if no records exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/RolePermission
        /// </code>
        /// </remarks>
        /// <returns>List of Role Permissions records</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAccesses()
        {
            try { 

                var rolepermission = await _rolepermissionService.GetAllAsync();
                if (!(rolepermission?.Data?.Any() ?? false))
                    return NotFound("No Access Records Found");

                return Ok(rolepermission);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Retrieve an Role Permissions record by ID
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Fetches a specific Role Permissions record using its unique identifier.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - Unique ID of the Role Permissions record</description></item>
        /// </list>  
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Returns the Role Permissions record if found</description></item>
        ///     <item><description>Returns 404 if the record does not exist</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// GET /AttendanceManagement/RolePermission/1
        /// </code>
        /// </remarks>
        /// <param name="id">RolePermission ID</param>
        /// <returns>RolePermission record</returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRolePermissionById(int id)
        {
            try { 
                var rolepermission = await _rolepermissionService.GetByIdAsync(id);
                if (rolepermission == null)
                    return NotFound($"#404! RolePermission with ID {id} not found");

                return Ok(rolepermission);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Create a new Role Permissions record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Creates a new Role Permissions entry in the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>Name</b> - Name of the Role Permissions</description></item>
        ///     <item><description><b>Description</b> - Optional description</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Creates a new Role Permissions record</description></item>
        /// </list>
        ///
        /// <para><b>Example:</b></para>
        /// <code>
        /// POST /AttendanceManagement/RolePermission
        /// {
        ///   "name": "Read",
        ///   "description": "Read-only Role Permissions"
        /// }
        /// </code>
        /// </remarks>
        /// <param name="dto">RolePermission data</param>
        /// <returns>Created Role Permissions record</returns>
        [HttpPost]
        public async Task<IActionResult> AddRolePermission(AddRolePermissionDTO dto)
        {
            try { 

                var rolepermissions = await _rolepermissionService.AddAsync(dto);
                return Ok(rolepermissions);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Update an existing Role Permissions record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Updates an existing Role Permissions record using its ID.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - RolePermission ID</description></item>
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
        /// <param name="id">RolePermission ID</param>
        /// <param name="dto">Updated Role Permissions data</param>
        /// <returns>Updated Role Permissions record</returns>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRolePermission(int id, AddRolePermissionDTO dto)
        {
            try { 
                var updatedRolePermission = await _rolepermissionService.UpdateAsync(id, dto);
                if (updatedRolePermission == null)
                    return NotFound($"#404! RolePermission with ID {id} not found");

                return Ok(updatedRolePermission);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Delete an Role Permissions record
        /// </summary>
        /// <remarks>
        /// <para><b>Description:</b></para>
        /// <para>Deletes an Role Permissions record from the system.</para>
        ///
        /// <para><b>Inputs:</b></para>
        /// <list type="bullet">
        ///     <item><description><b>id</b> - RolePermission ID to delete</description></item>
        /// </list>
        ///
        /// <para><b>Behavior:</b></para>
        /// <list type="bullet">
        ///     <item><description>Deletes the record if found</description></item>
        ///     <item><description>Returns 404 if not found</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">RolePermission ID</param>
        /// <returns>Status message</returns>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRolePermission(int id)
        {
            try
            {
                var success = await _rolepermissionService.DeleteAsync(id);
                if (!success)
                    return NotFound($"#404! RolePermission with ID {id} not found");

                return Ok($"RolePermission with ID {id} deleted successfully");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}