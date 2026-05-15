using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class AttendanceStudentRepository(DatabaseLibrary db) : IAttendanceStudentRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<AttendanceStudent>> GetAllAsync()
        {
            return await _db.AttendanceStudents.Include(a => a.Attendance).Include(a => a.Student).ToListAsync();
        }

        public async Task<AttendanceStudent?> GetByIdAsync(int attendanceId,string studentId)
        {
            return await _db.AttendanceStudents
                .Include(a => a.Attendance)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a =>a.Attendance_Id == attendanceId && a.Student_Id == studentId);
        }

        public async Task AddAsync(AttendanceStudent enrollment)
        {
            _db.AttendanceStudents.Add(enrollment);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(AttendanceStudent enrollment)
        {
            _db.Entry(enrollment).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(AttendanceStudent enrollment)
        {
            _db.AttendanceStudents.Remove(enrollment);
            await _db.SaveChangesAsync();
        }
    }
}