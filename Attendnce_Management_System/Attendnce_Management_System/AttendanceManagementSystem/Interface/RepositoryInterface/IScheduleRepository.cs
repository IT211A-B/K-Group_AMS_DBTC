using Attendance_Management_System.AttendanceManagementSystem.Model;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task<Schedule?> GetByIdAsync(int id);
        Task AddAsync(Schedule schedule);
        Task UpdateAsync(Schedule schedule);
        Task DeleteAsync(Schedule schedule);
    }
}