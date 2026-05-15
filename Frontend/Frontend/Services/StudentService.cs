<<<<<<< HEAD:Frontend/Services/StudentService.cs
namespace Frontend.Services
{
    public class StudentService
    {
        public bool IsSessionValid(string? role) => !string.IsNullOrEmpty(role);
        public bool IsStudent(string? role) => role == "student";
    }
}
=======
﻿namespace Frontend.Services
{
    public class StudentService
    {
        public bool IsSessionValid(string? role)
        {
            return !string.IsNullOrEmpty(role);
        }

        public bool IsStudent(string? role)
        {
            return role == "student";
        }
    }
}
>>>>>>> e184fcbcfe06e47564902f542f8e3d52da1323aa:Frontend/Frontend/Services/StudentService.cs
