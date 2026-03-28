using Attendance_Management_System.AttendanceManagementSystem;
using Attendance_Management_System.AttendanceManagementSystem.Model;
using Microsoft.EntityFrameworkCore;
using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Repository
{
    public class ProgramRepository(DatabaseLibrary db) : IProgramRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Program_>> GetAllAsync()
        {
            return await _db.Programs.ToListAsync();
        }

        public async Task<Program_?> GetByIdAsync(int id)
        {
            return await _db.Programs.FindAsync(id);
        }

        public async Task AddAsync(Program_ program)
        {
            _db.Programs.Add(program);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Program_ program)
        {
            _db.Entry(program).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Program_ program)
        {
            _db.Programs.Remove(program);
            await _db.SaveChangesAsync();
        }
    }
}