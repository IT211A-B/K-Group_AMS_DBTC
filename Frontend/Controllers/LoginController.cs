using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
            var role = HttpContext.Session.GetString("UserRole");
            if (!string.IsNullOrEmpty(role))
            {
                return role switch
                {
                    "admin" => Redirect("/Admin/Dashboard"),
                    "teacher" => Redirect("/Teacher/Dashboard"),
                    "student" => Redirect("/Student/Profile"),
                    _ => View()
                };
            }
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> Authenticate([FromBody] LoginViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest(new { message = "Email and password are required." });

            var (success, role, name, userId, token, errorMessage) =
                await _authService.AuthenticateAsync(model);

            if (success)
            {
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserName", name);
                HttpContext.Session.SetString("UserId", userId);
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("JwtToken", token);

                return Ok(new { role, name, userId });
            }
            else if (errorMessage.StartsWith("Too many requests"))
            {
                return StatusCode(429, new { message = errorMessage });
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
            Response.Cookies.Delete(".DBTC.Session");
            return RedirectToAction("Index");
        }
    }
}