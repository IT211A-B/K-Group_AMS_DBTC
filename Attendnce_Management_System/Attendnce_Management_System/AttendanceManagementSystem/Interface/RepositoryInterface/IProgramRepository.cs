using Attendance_Management_System.AttendanceManagementSystem.Model;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface
{
    public interface IProgramRepository
    {
        Task<IEnumerable<Program_>> GetAllAsync();
        Task<Program_?> GetByIdAsync(int id);
        Task AddAsync(Program_ program);
        Task UpdateAsync(Program_ program);
        Task DeleteAsync(Program_ program);
    }
}