using Microsoft.AspNetCore.Mvc;
using Frontend.Services;
using Frontend.Models;

namespace Frontend.Controllers
{
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;

        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        private IActionResult? CheckSession()
        {
            var role = HttpContext.Session.GetString("UserRole");
            if (!_adminService.IsSessionValid(role)) return Redirect("/Login/Index");
            if (!_adminService.IsAdmin(role)) return Redirect("/Login/Index");
            return null;
        }

        public IActionResult Dashboard()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new AdminViewModel { CurrentPage = "Dashboard" };
            return View(vm);
        }

        public IActionResult AccountHistory()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new AdminViewModel { CurrentPage = "AccountHistory" };
            return View(vm);
        }

        public IActionResult Courses()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new AdminViewModel { CurrentPage = "Courses" };
            return View(vm);
        }

        public IActionResult Profile()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new AdminViewModel { CurrentPage = "Profile" };
            return View(vm);
        }

        public IActionResult Mail()
        {
            var check = CheckSession(); if (check != null) return check;
            var vm = new AdminViewModel { CurrentPage = "Mail" };
            return View(vm);
        }

        public IActionResult ViewStudentProfile(string userId)
        {
            var check = CheckSession(); if (check != null) return check;
            ViewData["TargetUserId"] = userId;
            ViewData["IsAdminView"] = true;
            return View("~/Views/Student/Profile.cshtml", new AdminViewModel { CurrentPage = "Dashboard" });
        }

        public IActionResult ViewTeacherProfile(string userId)
        {
            var check = CheckSession(); if (check != null) return check;
            ViewData["TargetUserId"] = userId;
            ViewData["IsAdminView"] = true;
            return View("~/Views/Teacher/Profile.cshtml", new AdminViewModel { CurrentPage = "Dashboard" });
        }
    }
}