using Backend.Backend;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class AttendanceRepository(DatabaseLibrary db) : IAttendanceRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Attendance>> GetAllAsync()
        {
            return await _db.Attendances.ToListAsync();
        }

        public async Task<Attendance?> GetByIdAsync(int ID)
        {
            return await _db.Attendances.Include(a => a.Schedule).FirstOrDefaultAsync(a => a.Attendance_ID == ID);
        }

        public async Task AddAsync(Attendance attendance)
        {
            _db.Attendances.Add(attendance);
            await _db.SaveChangesAsync();
        }

        public async Task<Attendance?> GetAttendanceIfExist(string id, DateOnly today, TimeOnly now)
        {
            var windowStart = now.AddMinutes(-30);

            return await _db.Attendances
                .FromSqlRaw(@"
                    SELECT a.*
                    FROM ""Attendances"" a
                    JOIN ""Schedules"" sc ON sc.""Schedule_Id"" = a.""Schedule_ID""
                    JOIN ""Students"" st ON sc.""Section_ID"" = st.""SectionID""
                    WHERE st.""Student_ID"" = {0}
                    AND a.""Date"" = {1}
                    AND sc.""StartTime"" <= {2}
                    AND sc.""EndTime"" >= {2}", id, today, now)
                .Include(a=>a.Schedule)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _db.Entry(attendance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Attendance attendance)
        {
            _db.Attendances.Remove(attendance);
            await _db.SaveChangesAsync();
        }
    }
}