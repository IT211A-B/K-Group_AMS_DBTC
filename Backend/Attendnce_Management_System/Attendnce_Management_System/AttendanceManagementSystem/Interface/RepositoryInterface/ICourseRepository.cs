using Attendance_Management_System.AttendanceManagementSystem.Model;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int ID);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Course course);
    }
}