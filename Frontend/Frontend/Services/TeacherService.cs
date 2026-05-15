namespace Frontend.Services
{
    public class TeacherService
    {
        public bool IsSessionValid(string? role)
        {
            return !string.IsNullOrEmpty(role);
        }

        public bool IsTeacher(string? role)
        {
            return role == "teacher";
        }
    }
}