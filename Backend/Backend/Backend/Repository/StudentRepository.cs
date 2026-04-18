using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Backend.Backend.Repository;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Backend.Backend.Repository
{
    public class StudentRepository(DatabaseLibrary db) : IStudentRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _db.Students.ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int ID)
        {
            return await _db.Students
                .FromSqlRaw(@"SELECT * FROM ""Students"" 
                  WHERE CAST(SPLIT_PART(""DocumentSeries"", '-', 3) AS INT) = {0}", ID)
                .FirstOrDefaultAsync();
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

        public async Task<Program_?> GetProgramByIdAsync(int id)
        {
            return await _db.Programs.FindAsync(id);
        }

        public async Task<long> GetNextStudentNumber()
        {
            return await _db.Database
                .SqlQuery<long>($"SELECT nextval('StudentSeq') AS \"Value\"")
                .SingleAsync();
        }

        public async Task<bool> CheckUserIfTaken(int uId)
        {
            return await _db.Students.AnyAsync(s => s.User_ID == uId);
        }

    }
}