using Attendnce_Management_System.AttendanceManagementSystem;
using Attendnce_Management_System.AttendanceManagementSystem.Model;
using Microsoft.EntityFrameworkCore;
using Smart_Library.SmartLibraryManagement.Interface;

namespace Smart_Library.SmartLibraryManagement.Repository
{
    public class AttendanceRepository(DatabaseLibrary db) : IAttendanceRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Attendance>> GetAllAsync()
        {
            return await _db.Attendance.ToListAsync();
        }

        public async Task<Attendance?> GetByIdAsync(int id)
        {
            return await _db.Attendance.FindAsync(id);
        }

        public async Task AddAsync(Attendance attendance)
        {
            _db.Attendance.Add(attendance);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _db.Entry(attendance).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Attendance attendance)
        {
            _db.Attendance.Remove(attendance);
            await _db.SaveChangesAsync();
        }
    }
}