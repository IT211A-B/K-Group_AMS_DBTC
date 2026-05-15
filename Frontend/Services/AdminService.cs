namespace Frontend.Services
{
    public class AdminService
    {
        public bool IsSessionValid(string? role) => !string.IsNullOrEmpty(role);
        public bool IsAdmin(string? role) => role == "admin";
    }
}
