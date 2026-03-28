using Backend.Backend;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Repository
{
    public class ScheduleRepository(DatabaseLibrary db) : IScheduleRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _db.Schedules.ToListAsync();
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _db.Schedules.FindAsync(id);
        }

        public async Task AddAsync(Schedule schedule)
        {
            _db.Schedules.Add(schedule);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Schedule schedule)
        {
            _db.Entry(schedule).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Schedule schedule)
        {
            _db.Schedules.Remove(schedule);
            await _db.SaveChangesAsync();
        }
    }
}