using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    /// <summary>
    /// Controller responsible for handling authentication endpoints.
    /// </summary>
    /// <remarks>
    /// Provides APIs for user login and authentication.
    /// Delegates business logic to <see cref="IAuthService"/>.
    /// </remarks>
  
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Service used to handle authentication logic.
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor for AuthController.
        /// </summary>
        /// <param name="authService">
        /// Service responsible for authenticating users and generating tokens.
        /// </param>
        /// 

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Authenticates a user and returns a response indicating success or failure.
        /// </summary>
        /// <param name="logindto">
        /// DTO containing user login credentials (email/username and password).
        /// </param>
        /// <returns>
        /// Returns:
        /// - 200 OK if login is successful  
        /// - 401 Unauthorized if credentials are invalid  
        /// - 400 BadRequest if an unexpected error occurs  
        /// </returns>
        /// <remarks>
        /// Process flow:
        /// 1. Receives login request.
        /// 2. Calls AuthService to validate credentials.
        /// 3. Returns appropriate HTTP response based on result.
        /// </remarks>
        [HttpPost("/LogIn")]
        public async Task<IActionResult> Login(LoginUserDto logindto)
        {
            try
            {
                // Calls the authentication service to process login.
                var login = await _authService.LoginAsync(logindto);

                //Returns 401 Unauthorized if login fails.
                if (!login.isSuccess) return Unauthorized(login.Detail);

                // Returns 200 OK if login succeeds.
                return Ok(login);
            }
            catch (Exception x)
            {
                // Handles unexpected errors and returns 400 BadRequest.
                return BadRequest($"An Error \"{x}\" Occured");
            }
        }
    }
}