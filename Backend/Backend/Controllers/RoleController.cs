using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controllers
{
    /// <summary>
    /// Handles all operations related to roles.
    /// </summary>
    [Route("AttendanceManagement/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Retrieves all role records.
        /// </summary>
        /// <remarks>
        /// Returns a list of all roles available in the system.
        /// </remarks>
        /// <returns>List of role records</returns>
        /// <response code="200">Roles retrieved successfully</response>
        /// <response code="404">No roles found</response>
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            try { 
                var roles = await _roleService.GetAllAsync();
                if (!(roles?.Data?.Any() ?? false))
                    return NotFound("No Roles Found");

                return Ok(roles);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Retrieves a specific role by ID.
        /// </summary>
        /// <param name="id">The ID of the role</param>
        /// <returns>A single role record</returns>
        /// <response code="200">Role found</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            try { 
                var role = await _roleService.GetByIdAsync(id);
                if (role == null)
                    return NotFound($"#404! Role with ID {id} not found");

                return Ok(role);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Adds a new role.
        /// </summary>
        /// <param name="dto">Role data to create</param>
        /// <returns>The created role</returns>
        /// <response code="200">Role created successfully</response>
        [HttpPost]
        public async Task<IActionResult> AddRole(AddRoleDTO dto)
        {
            try { 
                var role = await _roleService.AddAsync(dto);
                return Ok(role);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        /// <param name="id">The ID of the role</param>
        /// <param name="dto">Updated role data</param>
        /// <returns>The updated role</returns>
        /// <response code="200">Role updated successfully</response>
        /// <response code="404">Role not found</response>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRole(int id, AddRoleDTO dto)
        {
            try { 
                var updatedRole = await _roleService.UpdateAsync(id, dto);
                if (updatedRole == null)
                    return NotFound($"#404! Role with ID {id} not found");

                return Ok(updatedRole);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        /// <summary>
        /// Deletes a role.
        /// </summary>
        /// <param name="id">The ID of the role</param>
        /// <returns>Confirmation message</returns>
        /// <response code="200">Role deleted successfully</response>
        /// <response code="404">Role not found</response>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            try { 
                var success = await _roleService.DeleteAsync(id);
                if (!success)
                    return NotFound($"#404! Role with ID {id} not found");

                return Ok($"Role with ID {id} deleted successfully");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}