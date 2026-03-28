using Backend.Backend.Model;

namespace Backend.Backend.Interface.ServiceInterface
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