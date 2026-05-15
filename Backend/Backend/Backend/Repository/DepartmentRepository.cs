using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class DepartmentRepository(DatabaseLibrary db) : IDepartmentRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _db.Departments.ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _db.Departments.FirstOrDefaultAsync(d => d.Department_Id == id);
        }

        public async Task AddAsync(Department department)
        {
            _db.Departments.Add(department);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            _db.Entry(department).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Department department)
        {
            _db.Departments.Remove(department);
            await _db.SaveChangesAsync();
        }
    }
}