using Microsoft.AspNetCore.Mvc;
using Frontend.Services;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class StudentController : Controller
    {
        private readonly StudentService _studentService;

        public StudentController(StudentService studentService)
        {
            _studentService = studentService;
        }

        private IActionResult? CheckSession()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (!_studentService.IsSessionValid(role)) return Redirect("/Login/Index");
            if (role == "admin") return Redirect("/Admin/Dashboard");
            if (role == "teacher") return Redirect("/Teacher/Dashboard");
            if (role != "student") return Redirect("/Login/Index");
            return null;
        }

        public IActionResult Index()
        {
            var check = CheckSession(); if (check != null) return check;
            return RedirectToAction("Profile");
        }

        public IActionResult Profile()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new StudentViewModel { CurrentPage = "Profile" };
            return View(vm);
        }

        public IActionResult Courses()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new StudentViewModel { CurrentPage = "Courses" };
            return View(vm);
        }

        public IActionResult Mail()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new StudentViewModel { CurrentPage = "Mail" };
            return View(vm);
        }
    }
}