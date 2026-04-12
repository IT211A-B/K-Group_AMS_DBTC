using Backend.Backend;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Repository
{
    public class UserRepository(DatabaseLibrary db) : IUserRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _db.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users.FindAsync(id);
        }

        public async Task<User?> GetByEmailOrUsernameAsync(string emailOrUsername)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Email == emailOrUsername || u.Full_Name == emailOrUsername);
        }

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _db.Entry(user).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task<long> GetNextUserNumberAsync()
        {
            var result = await _db.Database
                .SqlQuery<long>($"SELECT nextval('UserSeq') AS \"Value\"")
                .SingleAsync();
            return result;
        }
    }
}