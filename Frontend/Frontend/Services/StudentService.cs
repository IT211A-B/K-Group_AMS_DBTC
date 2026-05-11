namespace Frontend.Services
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