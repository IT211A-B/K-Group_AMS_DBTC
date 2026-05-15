using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IMailService
    {
        Task<ResponseDTO<IEnumerable<GetMailDTO>>> GetInboxAsync(string userId, string role);
        Task<ResponseDTO<GetMailDTO>> SendAsync(SendMailDTO dto, string senderUserId, string senderName, string senderRole);
        Task<ResponseDTO<IEnumerable<GetMailDTO>>> GetNotificationsAsync(string userId, string role);
        Task MarkReadAsync(int notificationId);
        Task MarkAllReadAsync(string userId);
        Task ClearAsync(string userId);
    }
}
