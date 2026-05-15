using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface ISectionRepository
    {
        Task<IEnumerable<Section>> GetAllAsync();
        Task<Section?> GetByIdAsync(int id);
        Task AddAsync(Section enrollment);
        Task UpdateAsync(Section enrollment);
        Task DeleteAsync(Section enrollment);
    }
}