using Attendance_Management_System.AttendanceManagementSystem.Model;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAllAsync();
        Task<Attendance?> GetByIdAsync(int ID);
        Task AddAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
        Task DeleteAsync(Attendance attendance);
    }
}