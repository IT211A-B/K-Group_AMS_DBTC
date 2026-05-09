using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Backend.Controller
{ 

    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try { 
                var users = await _userService.GetAllAsync();
                if (!(users.Data?.Any() ?? false))
                    return NotFound("No users found.");

                return Ok(users);
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
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                return Ok(user);
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(AddUserDTO dto)
        {
            try
            {
                var user = await _userService.AddAsync(dto);
                
                // If Email Already Exist
                if (user.StatusCode == 422)
                {
                    return UnprocessableEntity($"Unprocessable Entity-{user.StatusCode}: Email Already Exist");
                }
                //If The Email Does Not Follow the Given Email Domain, User Creation is Forbidden
                if (user.StatusCode == 403)
                {
                    return Forbid($"Forbidden-{user.StatusCode}: Given Email Does Not Contain a Role and Will Not Accepted");
                }

                return Ok(user);
            }catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, AddUserDTO dto)
        //{
        //    try { 
        //        var user = await _userService.UpdateAsync(id, dto);
        //        if (user == null)
        //            return NotFound($"User with ID {id} not found.");

        //        return Ok(user);
        //    }
        //    catch (Exception x)
        //    {
        //        // Internal Error
        //        return BadRequest($"An Error \"{x}\" Occured");
        //    }
        //}

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try { 
                var success = await _userService.DeleteAsync(id);
                if (!success)
                    return NotFound($"User with ID {id} not found.");

                return Ok($"User with ID {id} deleted successfully.");
            }
            catch (Exception x)
            {
                // Internal Error
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}