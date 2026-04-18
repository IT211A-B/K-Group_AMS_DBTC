using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Backend.Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
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
            return await _db.Teachers
                .FromSqlRaw(@"SELECT * FROM ""Students"" 
                  WHERE CAST(SPLIT_PART(""DocumentSeries"", '-', 3) AS INT) = {0}", ID)
                .FirstOrDefaultAsync();
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

        public async Task<Department?> GetDepartmentById(int ID)
        {
            return await _db.Departments.FindAsync(ID);
        }

        public async Task<long> GetNextTeacherNumberAsync()
        {
            return await _db.Database
                .SqlQuery<long>($"SELECT nextval('TeacherSeq') AS \"Value\"")
                .SingleAsync();
        }
    }
}