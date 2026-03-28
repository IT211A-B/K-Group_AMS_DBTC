using Attendance_Management_System.AttendanceManagementSystem.Model;

namespace Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface
{
    public interface IScheduleService
    {
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task<Schedule?> GetByIdAsync(int id);
        Task<Schedule> AddAsync(Schedule schedule);
        Task<Schedule?> UpdateAsync(int id, Schedule schedule);
        Task<bool> DeleteAsync(int id);
    }
}