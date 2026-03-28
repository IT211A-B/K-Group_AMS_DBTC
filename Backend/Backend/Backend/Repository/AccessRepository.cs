using Backend.Backend;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class AccessRepository(DatabaseLibrary db) : IAccessRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Access>> GetAllAsync()
        {
            return await _db.Accesses.ToListAsync();
        }

        public async Task<Access?> GetByIdAsync(int ID)
        {
            return await _db.Accesses.FindAsync(ID);
        }

        public async Task AddAsync(Access access)
        {
            _db.Accesses.Add(access);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Access access)
        {
            _db.Entry(access).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Access access)
        {
            _db.Accesses.Remove(access);
            await _db.SaveChangesAsync();
        }
    }
}