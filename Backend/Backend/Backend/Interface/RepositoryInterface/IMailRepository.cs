using Backend.Backend.Model;

namespace Backend.Backend.Interface.RepositoryInterface
{
    public interface IMailRepository
    {
        Task<IEnumerable<Mail>> GetInboxForUserAsync(string userId, string role);
        Task<Mail?> GetByIdAsync(int id);
        Task AddAsync(Mail mail);
        Task AddNotificationAsync(Notification notification);
        Task<IEnumerable<Notification>> GetNotificationsForUserAsync(string userId, string role);
        Task MarkNotificationReadAsync(int id);
        Task MarkAllNotificationsReadAsync(string userId);
        Task ClearNotificationsForUserAsync(string userId);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleTarget);
    }
}
