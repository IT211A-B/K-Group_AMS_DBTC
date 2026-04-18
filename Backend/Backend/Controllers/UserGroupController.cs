using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserGroupController : ControllerBase
    {
        private readonly IUserGroupService _userGroupService;

        public UserGroupController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try { 
                var groups = await _userGroupService.GetAllAsync();
                if (!(groups.Data?.Any() ?? false))
                    return NotFound("No user groups found.");

                return Ok(groups);
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
                var group = await _userGroupService.GetByIdAsync(id);
                if (group == null)
                    return NotFound($"User group with ID {id} not found.");

                return Ok(group);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserGroupDTO dto)
        {
            try { 
                var group = await _userGroupService.AddAsync(dto);
                return Ok(group);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AddUserGroupDTO dto)
        {
            try { 
                var group = await _userGroupService.UpdateAsync(id, dto);
                if (group == null)
                    return NotFound($"User group with ID {id} not found.");

                return Ok(group);
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
                var success = await _userGroupService.DeleteAsync(id);
                if (!success)
                    return NotFound($"User group with ID {id} not found.");

                return Ok($"User group with ID {id} deleted successfully.");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}