using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Backend.Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class PermissionRepository(DatabaseLibrary db) : IPermissionRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            return await _db.Permissions.ToListAsync();
        }

        public async Task<Permission?> GetByIdAsync(int ID)
        {
            return await _db.Permissions.FindAsync(ID);
        }

        public async Task AddAsync(Permission permission)
        {
            _db.Permissions.Add(permission);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Permission permission)
        {
            _db.Entry(permission).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Permission permission)
        {
            _db.Permissions.Remove(permission);
            await _db.SaveChangesAsync();
        }
    }
}