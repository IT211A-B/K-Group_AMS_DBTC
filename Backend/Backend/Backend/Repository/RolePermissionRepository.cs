using Backend.Backend;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class RolePermissionRepository(DatabaseLibrary db) : IRolePermissionRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<RolePermission>> GetAllAsync()
        {
            return await _db.RolePermissions.ToListAsync();
        }

        public async Task<RolePermission?> GetByIdAsync(int ID)
        {
            return await _db.RolePermissions.FindAsync(ID);
        }

        public async Task AddAsync(RolePermission rolepermissions)
        {
            _db.RolePermissions.Add(rolepermissions);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(RolePermission rolepermissions)
        {
            _db.Entry(rolepermissions).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(RolePermission rolepermissions)
        {
            _db.RolePermissions.Remove(rolepermissions);
            await _db.SaveChangesAsync();
        }
    }
}