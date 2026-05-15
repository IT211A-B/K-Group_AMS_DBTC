using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IDashboardRepository
    {
        Task<DashboardStatsDTO> GetStatsAsync();
    }
}
