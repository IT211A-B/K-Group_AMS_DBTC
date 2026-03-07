using Attendnce_Management_System.AttendanceManagementSystem;
using Attendnce_Management_System.AttendanceManagementSystem.Model;
using Microsoft.EntityFrameworkCore;
using Smart_Library.SmartLibraryManagement.Interface;

namespace Smart_Library.SmartLibraryManagement.Repository
{
    public class TeacherRepository(DatabaseLibrary db) : ITeacherRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            return await _db.Teachers.ToListAsync();
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {
            return await _db.Teachers.FindAsync(id);
        }

        public async Task AddAsync(Teacher teacher)
        {
            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            _db.Entry(teacher).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Teacher teacher)
        {
            _db.Teachers.Remove(teacher);
            await _db.SaveChangesAsync();
        }
    }
}