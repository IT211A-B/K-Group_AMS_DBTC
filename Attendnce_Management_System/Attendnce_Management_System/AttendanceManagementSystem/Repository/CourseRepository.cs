using Attendnce_Management_System.AttendanceManagementSystem;
using Attendnce_Management_System.AttendanceManagementSystem.Model;
using Microsoft.EntityFrameworkCore;
using Smart_Library.SmartLibraryManagement.Interface;

namespace Smart_Library.SmartLibraryManagement.Repository
{
    public class CourseRepository(DatabaseLibrary db) : ICourseRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _db.Courses.ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _db.Courses.FindAsync(id);
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