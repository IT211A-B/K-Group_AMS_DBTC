using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;
using Attendance_Management_System.AttendanceManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace Attendance_Management_System.AttendanceManagementSystem.Repository
{
    public class EnrollmentRepository(DatabaseLibrary db) : IEnrollmentRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _db.Enrollments.ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _db.Enrollments.FindAsync(id);
        }

        public async Task AddAsync(Enrollment enrollment)
        {
            _db.Enrollments.Add(enrollment);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            _db.Entry(enrollment).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Enrollment enrollment)
        {
            _db.Enrollments.Remove(enrollment);
            await _db.SaveChangesAsync();
        }
    }
}