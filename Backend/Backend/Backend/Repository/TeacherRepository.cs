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
            return await _db.Teachers.Include(t => t.User).Include(t => t.Courses).ToListAsync();
        }

        public async Task<Teacher?> GetByIdAsync(int ID)
        {
            return await _db.Teachers
                .FromSqlRaw(@"SELECT * FROM ""Teachers"" 
                  WHERE CAST(SPLIT_PART(""DocumentSeries"", '-', 3) AS INT) = {0}", ID)
                .Include(t => t.User).Include(t => t.Courses).FirstOrDefaultAsync();
        }

        public async Task<Teacher?> GetByQrToken(string qrToken)
        {
            return await _db.Teachers
                .Include(s => s.User)
                .Where(s => s.QrToken == qrToken)
                .FirstOrDefaultAsync();
        }

        public async Task<Teacher?> GetTeacherByUserUUIDAsync(string uId)
        {
            return await _db.Teachers.FirstOrDefaultAsync(t => t.User_ID == uId);;
        }
        public async Task<Teacher?> GetByUUIDAsync(string id)
        {
            return await _db.Teachers
                .Include(t => t.User).Include(t => t.Courses).FirstOrDefaultAsync(fid => fid.Teacher_ID == id);
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