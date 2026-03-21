using Attendance_Management_System.AttendanceManagementSystem.Model;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int ID);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
    }
}
