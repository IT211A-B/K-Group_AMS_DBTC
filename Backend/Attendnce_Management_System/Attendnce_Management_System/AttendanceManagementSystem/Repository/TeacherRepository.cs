using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;
using Attendance_Management_System.AttendanceManagementSystem.Model;
using Attendance_Management_System.AttendanceManagementSystem.Repository;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management_System.AttendanceManagementSystem.Repository
{
    public class TeacherRepository(DatabaseLibrary db) : ITeacherRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            return await _db.Teachers.ToListAsync();
        }

        public async Task<Teacher?> GetByIdAsync(int ID)
        {
            return await _db.Teachers.FindAsync(ID);
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