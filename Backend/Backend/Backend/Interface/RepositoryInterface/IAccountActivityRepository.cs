using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IAccountActivityRepository
    {
        Task AddAsync(AccountActivity activity);
        Task<IEnumerable<AccountActivity>> GetRecentAsync(int limit = 200);
    }
}
