using Attendnce_Management_System.AttendanceManagementSystem.Model;

namespace Smart_Library.SmartLibraryManagement.Interface
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
    }
}
