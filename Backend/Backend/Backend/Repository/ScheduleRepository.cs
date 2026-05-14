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
            return await _db.Schedules.Include(s => s.Course)
            .Include(s => s.Section).ToListAsync();
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _db.Schedules.Include(s => s.Course)
            .Include(s => s.Section).FirstOrDefaultAsync(s => s.Schedule_Id == id);
        }

        public async Task AddAsync(Schedule schedule)
        {
            _db.Schedules.Add(schedule);
            await _db.SaveChangesAsync();
        }

        public async Task<Schedule?> GetScheduleIfExist(string id, DayOfWeek dayOfWeek, TimeOnly now)
        {
            var windowStart = now.AddMinutes(-30);

            return await _db.Schedules
                .FromSqlRaw(@"
                    SELECT s.*
                    FROM ""Schedules"" s
                    JOIN ""Courses"" c ON s.""Course_ID"" = c.""Course_ID""
                    JOIN ""Teachers"" st ON st.""Teacher_ID"" = c.""Teacher_ID""
                    WHERE st.""Teacher_ID"" = {0}
                    AND s.""DayOfWeek"" = {1}
                    AND s.""StartTime"" - INTERVAL '30 minutes' <= {2}
                    AND s.""EndTime"" >= {2}",
                    id, dayOfWeek, now)
                .Include(s => s.Course)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> HasConflictingScheduleAsync(int courseId,string academicYear,TimeOnly startTime,TimeOnly endTime,int sectionId)
        {
            return await _db.Schedules.AnyAsync(s =>
                s.Course_ID == courseId
                && s.AcademicYear == academicYear
                && s.StartTime == startTime
                && s.EndTime == endTime
                && s.Section_ID != sectionId);
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