using Microsoft.AspNetCore.Mvc;
using Frontend.Services;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthService _authService;
        private readonly SecureTokenStorage _tokenStorage;

        public LoginController(AuthService authService, SecureTokenStorage tokenStorage)
        {
            _authService = authService;
            _tokenStorage = tokenStorage;
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
        public async Task<IActionResult> Authenticate([FromBody] LoginViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                return BadRequest(new { message = "Email and password are required." });

            var (success, role, name, userId, token, errorMessage) =
                await _authService.AuthenticateAsync(model);

            if (success)
            {
                await HttpContext.Session.LoadAsync();
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserName", name);
                HttpContext.Session.SetString("UserId", userId);
                HttpContext.Session.SetString("UserEmail", model.Email);
                await _tokenStorage.SaveAsync(token);
                await HttpContext.Session.CommitAsync();

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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Session.LoadAsync();
            HttpContext.Session.Clear();
            await _tokenStorage.ClearAsync();
            Response.Cookies.Delete(".DBTC.Session");
            return RedirectToAction("Index");
        }
    }
}