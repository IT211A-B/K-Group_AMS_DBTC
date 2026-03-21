using Attendance_Management_System.AttendanceManagementSystem;
using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;
using Attendance_Management_System.AttendanceManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management_System.AttendanceManagementSystem.Repository
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
            return await _db.Attendances.FindAsync(ID);
        }

        public async Task AddAsync(Attendance attendance)
        {
            _db.Attendances.Add(attendance);
            await _db.SaveChangesAsync();
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