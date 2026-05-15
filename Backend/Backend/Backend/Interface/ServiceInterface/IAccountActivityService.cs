using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IAccountActivityService
    {
        Task LogAsync(string? userId, string activityType, string description, string? actorUserId = null, string? actorName = null, int? relatedId = null);
        Task<ResponseDTO<IEnumerable<GetAccountActivityDTO>>> GetRecentAsync();
    }
}
