namespace Frontend.Services
{
    public class StudentService
    {
        public bool IsSessionValid(string? role) => !string.IsNullOrEmpty(role);
        public bool IsStudent(string? role) => role == "student";
    }
}
