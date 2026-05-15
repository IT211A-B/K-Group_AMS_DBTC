using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IDashboardService
    {
        Task<ResponseDTO<DashboardStatsDTO>> GetStatsAsync();
    }
}
