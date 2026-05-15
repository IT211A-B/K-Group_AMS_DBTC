using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;

namespace Backend.Backend.Service
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _dashboardRepository;

        public DashboardService(IDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<ResponseDTO<DashboardStatsDTO>> GetStatsAsync()
        {
            var stats = await _dashboardRepository.GetStatsAsync();
            return new ResponseDTO<DashboardStatsDTO> { Status_code = 200, Data = stats };
        }
    }
}
