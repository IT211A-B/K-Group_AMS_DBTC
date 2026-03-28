using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IAccessRepository
    {
        Task<IEnumerable<Access>> GetAllAsync();
        Task<Access?> GetByIdAsync(int ID);
        Task AddAsync(Access access);
        Task UpdateAsync(Access access);
        Task DeleteAsync(Access access);
    }
}