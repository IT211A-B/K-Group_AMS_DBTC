using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAllAsync();
        Task<Attendance?> GetByIdAsync(int ID);
        Task AddAsync(Attendance attendance);
        Task<Attendance?> GetAttendanceIfExist(string id, DateOnly today, TimeOnly now);
        Task<Attendance?> GetTodayByScheduleAsync(int scheduleId, DateOnly date);
        Task UpdateAsync(Attendance attendance);
        Task DeleteAsync(Attendance attendance);
    }
}