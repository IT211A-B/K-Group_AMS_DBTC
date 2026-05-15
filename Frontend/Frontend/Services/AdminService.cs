<<<<<<< HEAD:Frontend/Services/AdminService.cs
namespace Frontend.Services
{
    public class AdminService
    {
        public bool IsSessionValid(string? role) => !string.IsNullOrEmpty(role);
        public bool IsAdmin(string? role) => role == "admin";
    }
}
=======
﻿namespace Frontend.Services
{
    public class AdminService
    {
        public bool IsSessionValid(string? role)
        {
            return !string.IsNullOrEmpty(role);
        }

        public bool IsAdmin(string? role)
        {
            return role == "admin";
        }
    }
}
>>>>>>> e184fcbcfe06e47564902f542f8e3d52da1323aa:Frontend/Frontend/Services/AdminService.cs
