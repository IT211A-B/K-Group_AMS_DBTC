using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;

namespace Backend.Backend.Service
{
    public class AccountActivityService : IAccountActivityService
    {
        private readonly IAccountActivityRepository _repository;

        public AccountActivityService(IAccountActivityRepository repository)
        {
            _repository = repository;
        }

        public async Task LogAsync(string? userId, string activityType, string description, string? actorUserId = null, string? actorName = null, int? relatedId = null)
        {
            await _repository.AddAsync(new AccountActivity
            {
                User_Id = userId,
                Activity_Type = activityType,
                Description = description,
                Actor_User_Id = actorUserId,
                Actor_Name = actorName,
                Related_Id = relatedId,
                CreatedAt = DateTime.UtcNow
            });
        }

        public async Task<ResponseDTO<IEnumerable<GetAccountActivityDTO>>> GetRecentAsync()
        {
            var items = await _repository.GetRecentAsync();
            var data = items.Select(a => new GetAccountActivityDTO
            {
                Activity_Id = a.Activity_Id,
                User_Id = a.User_Id,
                Activity_Type = a.Activity_Type,
                Description = a.Description,
                Actor_Name = a.Actor_Name,
                CreatedAt = a.CreatedAt
            });
            return new ResponseDTO<IEnumerable<GetAccountActivityDTO>> { Status_code = 200, Data = data };
        }
    }
}
