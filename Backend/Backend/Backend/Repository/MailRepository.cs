using Backend.Backend;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class MailRepository(DatabaseLibrary db) : IMailRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Mail>> GetInboxForUserAsync(string userId, string role)
        {
            var roleLower = role.ToLowerInvariant();
            return await _db.Mails
                .Where(m =>
                    m.Recipient_Target == userId ||
                    m.Recipient_Target == "all" ||
                    (m.Recipient_Target == "teacher" && roleLower == "teacher") ||
                    (m.Recipient_Target == "student" && roleLower == "student") ||
                    (roleLower == "admin" && (m.Recipient_Target == "admin" || m.Sender_User_Id == userId)))
                .OrderByDescending(m => m.CreatedAt)
                .Take(100)
                .ToListAsync();
        }

        public async Task<Mail?> GetByIdAsync(int id) =>
            await _db.Mails.FindAsync(id);

        public async Task AddAsync(Mail mail)
        {
            _db.Mails.Add(mail);
            await _db.SaveChangesAsync();
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsForUserAsync(string userId, string role)
        {
            return await _db.Notifications
                .Include(n => n.Mail)
                .Where(n => n.User_Id == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(50)
                .ToListAsync();
        }

        public async Task MarkNotificationReadAsync(int id)
        {
            var n = await _db.Notifications.FindAsync(id);
            if (n != null)
            {
                n.IsRead = true;
                await _db.SaveChangesAsync();
            }
        }

        public async Task MarkAllNotificationsReadAsync(string userId)
        {
            await _db.Notifications
                .Where(n => n.User_Id == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }

        public async Task ClearNotificationsForUserAsync(string userId)
        {
            await _db.Notifications.Where(n => n.User_Id == userId).ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleTarget)
        {
            if (roleTarget == "all")
                return await _db.Users.ToListAsync();

            var prefix = roleTarget switch
            {
                "teacher" => "TEA",
                "student" => "STU",
                "admin" => "ADM",
                _ => ""
            };

            if (string.IsNullOrEmpty(prefix))
                return [];

            return await _db.Users
                .Where(u => u.DocumentSeries.StartsWith(prefix))
                .ToListAsync();
        }
    }
}
