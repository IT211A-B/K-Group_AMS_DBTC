using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Backend.Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class UserGroupRepository(DatabaseLibrary db) : IUserGroupRepository
    {
        private readonly DatabaseLibrary _db = db;   // DB Context

        public async Task<IEnumerable<UserGroup>> GetAllAsync()
        {
            return await _db.UserGroups.ToListAsync();   // Fetch all
        }

        public async Task<UserGroup?> GetByIdAsync(int ID)
        {
            return await _db.UserGroups.FindAsync(ID);   // Find by PK
        }

        public async Task AddAsync(UserGroup userGroup)
        {
            _db.UserGroups.Add(userGroup);               // Add
            await _db.SaveChangesAsync();                // Save
        }

        public async Task UpdateAsync(UserGroup userGroup)
        {
            _db.Entry(userGroup).State = EntityState.Modified; // Mark modified
            await _db.SaveChangesAsync();                      // Save
        }

        public async Task DeleteAsync(UserGroup userGroup)
        {
            _db.UserGroups.Remove(userGroup);          // Remove
            await _db.SaveChangesAsync();              // Save
        }
    }
}