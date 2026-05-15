using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Frontend.Filters
{
    public class SessionAuthFilter : IActionFilter
    {
        private readonly string[] _roles;

        public SessionAuthFilter(params string[] roles) => _roles = roles;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var role = session.GetString("UserRole") ?? "";
            if (string.IsNullOrEmpty(role))
            {
                context.Result = new RedirectResult("/Login/Index");
                return;
            }
            if (_roles.Length > 0 && !_roles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = role switch
                {
                    "admin" => new RedirectResult("/Admin/Dashboard"),
                    "teacher" => new RedirectResult("/Teacher/Dashboard"),
                    "student" => new RedirectResult("/Student/Profile"),
                    _ => new RedirectResult("/Login/Index")
                };
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }

    public class RequireAdminAttribute : TypeFilterAttribute
    {
        public RequireAdminAttribute() : base(typeof(SessionAuthFilter)) =>
            Arguments = new object[] { new[] { "admin" } };
    }

    public class RequireTeacherAttribute : TypeFilterAttribute
    {
        public RequireTeacherAttribute() : base(typeof(SessionAuthFilter)) =>
            Arguments = new object[] { new[] { "teacher" } };
    }

    public class RequireStudentAttribute : TypeFilterAttribute
    {
        public RequireStudentAttribute() : base(typeof(SessionAuthFilter)) =>
            Arguments = new object[] { new[] { "student" } };
    }

    public class RequireAuthenticatedAttribute : TypeFilterAttribute
    {
        public RequireAuthenticatedAttribute() : base(typeof(SessionAuthFilter)) =>
            Arguments = new object[] { Array.Empty<string>() };
    }
}
