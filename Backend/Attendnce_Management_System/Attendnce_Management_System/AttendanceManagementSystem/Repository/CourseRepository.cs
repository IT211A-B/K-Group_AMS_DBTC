using Attendance_Management_System.AttendanceManagementSystem;
using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;
using Attendance_Management_System.AttendanceManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management_System.AttendanceManagementSystem.Repository
{
    public class CourseRepository(DatabaseLibrary db) : ICourseRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _db.Courses.ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int ID)
        {
            return await _db.Courses.FindAsync(ID);
        }

        public async Task AddAsync(Course course)
        {
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _db.Entry(course).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Course course)
        {
            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
        }
    }
}