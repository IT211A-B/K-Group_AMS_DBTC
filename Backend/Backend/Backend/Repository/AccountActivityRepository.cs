using Backend.Backend;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class AccountActivityRepository(DatabaseLibrary db) : IAccountActivityRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task AddAsync(AccountActivity activity)
        {
            _db.AccountActivities.Add(activity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<AccountActivity>> GetRecentAsync(int limit = 200) =>
            await _db.AccountActivities
                .OrderByDescending(a => a.CreatedAt)
                .Take(limit)
                .ToListAsync();
    }
}
