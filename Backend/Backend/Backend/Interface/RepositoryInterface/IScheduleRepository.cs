using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IScheduleRepository
    {
        Task<IEnumerable<Schedule>> GetAllAsync();
        Task<Schedule?> GetByIdAsync(int id);
        Task AddAsync(Schedule schedule);
        Task<bool> HasConflictingScheduleAsync(int courseId, string academicYear, TimeOnly startTime, TimeOnly endTime, int sectionId);
        Task UpdateAsync(Schedule schedule);
        Task DeleteAsync(Schedule schedule);
    }
}