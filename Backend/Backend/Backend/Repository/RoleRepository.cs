using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Backend.Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class RoleRepository(DatabaseLibrary db) : IRoleRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _db.Roles.ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _db.Roles.FindAsync(id);
        }

        public async Task AddAsync(Role role)
        {
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            _db.Entry(role).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Role role)
        {
            _db.Roles.Remove(role);
            await _db.SaveChangesAsync();
        }
    }
}