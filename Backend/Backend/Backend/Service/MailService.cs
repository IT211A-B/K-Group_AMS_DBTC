using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;

namespace Backend.Backend.Service
{
    public class MailService : IMailService
    {
        private readonly IMailRepository _mailRepository;
        private readonly IAccountActivityRepository _activityRepository;

        public MailService(IMailRepository mailRepository, IAccountActivityRepository activityRepository)
        {
            _mailRepository = mailRepository;
            _activityRepository = activityRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetMailDTO>>> GetInboxAsync(string userId, string role)
        {
            var notifications = await _mailRepository.GetNotificationsForUserAsync(userId, role);
            var data = notifications.Select(n => new GetMailDTO
            {
                Id = n.Notification_Id,
                RecipientId = userId,
                SenderUserId = n.Mail?.Sender_User_Id ?? "",
                SenderName = n.Mail?.Sender_Name ?? "System",
                SenderRole = n.Mail?.Sender_Role ?? "",
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            });
            return new ResponseDTO<IEnumerable<GetMailDTO>> { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetMailDTO>> SendAsync(SendMailDTO dto, string senderUserId, string senderName, string senderRole)
        {
            var mail = new Mail
            {
                Sender_User_Id = senderUserId,
                Sender_Name = senderName,
                Sender_Role = senderRole,
                Recipient_Target = dto.RecipientId,
                Subject = dto.Title,
                Body = dto.Message,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow
            };
            await _mailRepository.AddAsync(mail);

            var recipients = await ResolveRecipientsAsync(dto.RecipientId);
            foreach (var recipient in recipients)
            {
                await _mailRepository.AddNotificationAsync(new Notification
                {
                    User_Id = recipient.Id,
                    Mail_Id = mail.Mail_Id,
                    Title = dto.Title,
                    Message = dto.Message,
                    Type = dto.Type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _activityRepository.AddAsync(new AccountActivity
            {
                User_Id = dto.RecipientId == "all" || dto.RecipientId == "teacher" || dto.RecipientId == "student"
                    ? null : dto.RecipientId,
                Activity_Type = "MailSent",
                Description = $"{senderName} sent mail: {dto.Title}",
                Actor_User_Id = senderUserId,
                Actor_Name = senderName,
                Related_Id = mail.Mail_Id,
                CreatedAt = DateTime.UtcNow
            });

            return new ResponseDTO<GetMailDTO>
            {
                Status_code = 200,
                Data = new GetMailDTO
                {
                    Id = mail.Mail_Id,
                    RecipientId = dto.RecipientId,
                    SenderUserId = senderUserId,
                    SenderName = senderName,
                    SenderRole = senderRole,
                    Title = dto.Title,
                    Message = dto.Message,
                    Type = dto.Type,
                    IsRead = false,
                    CreatedAt = mail.CreatedAt
                }
            };
        }

        public async Task<ResponseDTO<IEnumerable<GetMailDTO>>> GetNotificationsAsync(string userId, string role) =>
            await GetInboxAsync(userId, role);

        public async Task MarkReadAsync(int notificationId) =>
            await _mailRepository.MarkNotificationReadAsync(notificationId);

        public async Task MarkAllReadAsync(string userId) =>
            await _mailRepository.MarkAllNotificationsReadAsync(userId);

        public async Task ClearAsync(string userId) =>
            await _mailRepository.ClearNotificationsForUserAsync(userId);

        private async Task<List<User>> ResolveRecipientsAsync(string target)
        {
            if (target is "all" or "teacher" or "student")
                return (await _mailRepository.GetUsersByRoleAsync(target)).ToList();

            var user = (await _mailRepository.GetUsersByRoleAsync("all"))
                .FirstOrDefault(u => u.Id == target);
            return user != null ? [user] : [];
        }
    }
}
