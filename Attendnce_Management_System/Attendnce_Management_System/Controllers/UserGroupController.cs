using Microsoft.AspNetCore.Mvc;
using Attendance_Management_System.AttendanceManagementSystem.DTOs;
using Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Controller
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
            var groups = await _userGroupService.GetAllAsync();
            if (!groups.Any())
                return NotFound("No user groups found.");

            return Ok(groups);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var group = await _userGroupService.GetByIdAsync(id);
            if (group == null)
                return NotFound($"User group with ID {id} not found.");

            return Ok(group);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddUserGroupDTO dto)
        {
            var group = await _userGroupService.AddAsync(dto);
            return Ok(group);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AddUserGroupDTO dto)
        {
            var group = await _userGroupService.UpdateAsync(id, dto);
            if (group == null)
                return NotFound($"User group with ID {id} not found.");

            return Ok(group);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userGroupService.DeleteAsync(id);
            if (!success)
                return NotFound($"User group with ID {id} not found.");

            return Ok($"User group with ID {id} deleted successfully.");
        }
    }
}