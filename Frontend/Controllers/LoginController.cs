using Microsoft.AspNetCore.Mvc;
using Frontend.Services;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthService _authService;

        public LoginController(AuthService authService)
        {
            _authService = authService;
        }

        public IActionResult Index()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] LoginViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest(new { message = "Email and password are required." });

            var (success, role, name, userId, userGroupId, errorMessage) = await _authService.AuthenticateAsync(model);

            if (success)
            {
                // Set secure session cookies
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserName", name);
                HttpContext.Session.SetString("UserId", userId);
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("UserGroupId", userGroupId);

                // Return role so frontend can redirect
                return Ok(new { role, name, userId });
            }
            else if (errorMessage.StartsWith("Cannot connect"))
            {
                return StatusCode(502, new { message = errorMessage });
            }
            else
            {
                return Unauthorized(new { message = errorMessage });
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}