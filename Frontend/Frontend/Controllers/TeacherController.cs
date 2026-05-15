using Microsoft.AspNetCore.Mvc;
using Frontend.Services;
using Frontend.Models;
using Frontend.Filters;

namespace Frontend.Controllers
{
    [RequireTeacher]
    public class TeacherController : Controller
    {
        private readonly TeacherService _teacherService;

        public TeacherController(TeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        private IActionResult? CheckSession()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (!_teacherService.IsSessionValid(role)) return Redirect("/Login/Index");
            if (role == "admin") return Redirect("/Admin/Dashboard");
            if (role != "teacher") return Redirect("/Login/Index");
            return null;
        }

        public IActionResult Index()
        {
            var check = CheckSession(); if (check != null) return check;
            return RedirectToAction("Dashboard");
        }

        public IActionResult Dashboard()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new TeacherViewModel { CurrentPage = "Dashboard" };
            return View(vm);
        }

        public IActionResult Attendance()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new TeacherViewModel { CurrentPage = "Attendance" };
            return View(vm);
        }

        public IActionResult Profile()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new TeacherViewModel { CurrentPage = "Profile" };
            return View(vm);
        }

        public IActionResult Mail()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new TeacherViewModel { CurrentPage = "Mail" };
            return View(vm);
        }
    }
}