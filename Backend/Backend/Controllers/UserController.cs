using Microsoft.AspNetCore.Mvc;
using Backend.Backend.DTO;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Controller
{
    [Route("api/[controller]")]
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
            var users = await _userService.GetAllAsync();
            if (!users.Any())
                return NotFound("No users found.");

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register(AddUserDTO dto)
        {
            var user = await _userService.AddAsync(dto);
            return Ok(user);
        }

        [HttpPost("/LogIn")]
        public async Task<IActionResult> Login(LoginUserDto logindto)
        {
            var login = await _userService.LoginAsync(logindto);
            if (!login.isSuccess) return Unauthorized(login.Detail);

            return Ok(login.Detail);
        }

        //[HttpPut("{id:int}")]
        //public async Task<IActionResult> Update(int id, AddUserDTO dto)
        //{
        //    var user = await _userService.UpdateAsync(id, dto);
        //    if (user == null)
        //        return NotFound($"User with ID {id} not found.");

        //    return Ok(user);
        //}

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _userService.DeleteAsync(id);
            if (!success)
                return NotFound($"User with ID {id} not found.");

            return Ok($"User with ID {id} deleted successfully.");
        }
    }
}