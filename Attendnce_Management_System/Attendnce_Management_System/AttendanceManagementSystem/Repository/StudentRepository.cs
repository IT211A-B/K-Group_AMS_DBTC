using Attendnce_Management_System.AttendanceManagementSystem;
using Attendnce_Management_System.AttendanceManagementSystem.Model;
using Microsoft.EntityFrameworkCore;
using Smart_Library.SmartLibraryManagement.Interface;

namespace Smart_Library.SmartLibraryManagement.Repository
{
    public class StudentRepository(DatabaseLibrary db) : IStudentRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _db.Students.ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _db.Students.FindAsync(id);
        }

        public async Task AddAsync(Student student)
        {
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _db.Entry(student).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Student student)
        {
            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
        }
    }
}