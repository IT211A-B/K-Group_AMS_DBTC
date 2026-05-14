namespace Frontend.Services
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