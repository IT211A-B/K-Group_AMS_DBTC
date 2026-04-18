using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controllers
{
    /// <summary>
    /// Handles all operations related to permissions.
    /// </summary>
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// Retrieves all permission records.
        /// </summary>
        /// <remarks>
        /// Returns a list of all permissions available in the system.
        /// </remarks>
        /// <returns>List of permission records</returns>
        /// <response code="200">Permissions retrieved successfully</response>
        /// <response code="404">No permissions found</response>
        [HttpGet]
        public async Task<IActionResult> GetAllPermissions()
        {
            try { 
                var permissions = await _permissionService.GetAllAsync();
                if (!(permissions?.Data?.Any() ?? false))
                    return NotFound("No Permissions Found");

                return Ok(permissions);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Retrieves a specific permission by ID.
        /// </summary>
        /// <param name="id">The ID of the permission</param>
        /// <returns>A single permission record</returns>
        /// <response code="200">Permission found</response>
        /// <response code="404">Permission not found</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try { 
                var permission = await _permissionService.GetByIdAsync(id);
                if (permission == null)
                    return NotFound($"#404! Permission with ID {id} not found");

                return Ok(permission);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Adds a new permission.
        /// </summary>
        /// <param name="dto">Permission data to create</param>
        /// <returns>The created permission</returns>
        /// <response code="200">Permission created successfully</response>
        [HttpPost]
        public async Task<IActionResult> AddPermission(AddPermissionDTO dto)
        {
            try { 
                var permission = await _permissionService.AddAsync(dto);
                return Ok(permission);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Updates an existing permission.
        /// </summary>
        /// <param name="id">The ID of the permission</param>
        /// <param name="dto">Updated permission data</param>
        /// <returns>The updated permission</returns>
        /// <response code="200">Permission updated successfully</response>
        /// <response code="404">Permission not found</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePermission(int id, AddPermissionDTO dto)
        {
            try { 
                var updatedPermission = await _permissionService.UpdateAsync(id, dto);
                if (updatedPermission == null)
                    return NotFound($"#404! Permission with ID {id} not found");

                return Ok(updatedPermission);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Deletes a permission.
        /// </summary>
        /// <param name="id">The ID of the permission</param>
        /// <returns>Confirmation message</returns>
        /// <response code="200">Permission deleted successfully</response>
        /// <response code="404">Permission not found</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            try { 
                var success = await _permissionService.DeleteAsync(id);
                if (!success)
                    return NotFound($"#404! Permission with ID {id} not found");

                return Ok($"Permission with ID {id} deleted successfully");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}