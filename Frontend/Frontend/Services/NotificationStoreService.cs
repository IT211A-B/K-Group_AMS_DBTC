using System.Text.Json;
using System.Text.Json.Serialization;

namespace Frontend.Services
{
    public class NotificationStoreService
    {
        private readonly string _filePath;
        private readonly object _lock = new();
        private StoreData _data = new();

        public NotificationStoreService(IWebHostEnvironment env)
        {
            var dir = Path.Combine(env.ContentRootPath, "App_Data");
            Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "ams-store.json");
            Load();
        }

        public List<MsgItem> Notifications
        {
            get { lock (_lock) return _data.Notifications; }
        }

        public List<MailItem> Messages
        {
            get { lock (_lock) return _data.Messages; }
        }

        public void Save()
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
        }

        private void Load()
        {
            try
            {
                if (!File.Exists(_filePath)) return;
                var json = File.ReadAllText(_filePath);
                var loaded = JsonSerializer.Deserialize<StoreData>(json);
                if (loaded != null) _data = loaded;
            }
            catch { _data = new StoreData(); }
        }

        public int NextNotificationId()
        {
            lock (_lock)
            {
                _data.NotificationSeq++;
                return _data.NotificationSeq;
            }
        }

        public int NextMessageId()
        {
            lock (_lock)
            {
                _data.MessageSeq++;
                return _data.MessageSeq;
            }
        }

        public void AddNotification(MsgItem item)
        {
            lock (_lock)
            {
                _data.Notifications.Add(item);
                Save();
            }
        }

        public void AddMessage(MailItem item)
        {
            lock (_lock)
            {
                _data.Messages.Add(item);
                Save();
            }
        }

        public List<MsgItem> GetNotificationsFor(string role, string userId)
        {
            lock (_lock)
            {
                return _data.Notifications.Where(m =>
                    m.RecipientId == userId ||
                    m.RecipientId == "all" ||
                    (role == "admin" && (m.RecipientId == "admin" || m.RecipientId == "all")) ||
                    (role == "teacher" && (m.RecipientId == "teacher" || m.RecipientId == "all")) ||
                    (role == "student" && m.RecipientId == userId)
                ).OrderByDescending(m => m.CreatedAt).Take(50).ToList();
            }
        }

        public List<MailItem> GetInboxFor(string userId, string role)
        {
            lock (_lock)
            {
                return _data.Messages.Where(m =>
                    m.RecipientId == userId ||
                    m.RecipientId == "all" ||
                    (role == "admin" && m.RecipientId == "admin") ||
                    (role == "teacher" && m.RecipientId == "teacher") ||
                    (role == "student" && m.RecipientId == "student")
                ).OrderByDescending(m => m.CreatedAt).ToList();
            }
        }

        public List<MailItem> GetSentFor(string userId)
        {
            lock (_lock)
            {
                return _data.Messages.Where(m => m.SenderUserId == userId)
                    .OrderByDescending(m => m.CreatedAt).ToList();
            }
        }

        public void MarkNotificationRead(int id)
        {
            lock (_lock)
            {
                var item = _data.Notifications.FirstOrDefault(x => x.Id == id);
                if (item != null) item.IsRead = true;
                Save();
            }
        }

        public void MarkMessageRead(int id)
        {
            lock (_lock)
            {
                var item = _data.Messages.FirstOrDefault(x => x.Id == id);
                if (item != null) item.IsRead = true;
                Save();
            }
        }

        public void ClearNotificationsFor(string userId, string role)
        {
            lock (_lock)
            {
                _data.Notifications.RemoveAll(m =>
                    m.RecipientId == userId ||
                    m.RecipientId == "all" ||
                    (role == "admin" && m.RecipientId == "admin") ||
                    (role == "teacher" && m.RecipientId == "teacher"));
                Save();
            }
        }

        public void DeleteNotification(int id)
        {
            lock (_lock)
            {
                _data.Notifications.RemoveAll(m => m.Id == id);
                Save();
            }
        }

        public void ClearMessagesFor(string userId)
        {
            lock (_lock)
            {
                _data.Messages.RemoveAll(m => m.RecipientId == userId || m.SenderUserId == userId);
                Save();
            }
        }

        public int NotificationCount => Notifications.Count;

        private class StoreData
        {
            public int NotificationSeq { get; set; }
            public int MessageSeq { get; set; }
            public List<MsgItem> Notifications { get; set; } = new();
            public List<MailItem> Messages { get; set; } = new();
        }
    }

    public class MsgItem
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("recipientId")] public string RecipientId { get; set; } = "";
        [JsonPropertyName("senderUserId")] public string SenderUserId { get; set; } = "";
        [JsonPropertyName("senderName")] public string SenderName { get; set; } = "";
        [JsonPropertyName("senderRole")] public string SenderRole { get; set; } = "";
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("message")] public string Message { get; set; } = "";
        [JsonPropertyName("type")] public string Type { get; set; } = "info";
        [JsonPropertyName("isRead")] public bool IsRead { get; set; }
        [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }
    }

    public class MailItem
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("recipientId")] public string RecipientId { get; set; } = "";
        [JsonPropertyName("senderUserId")] public string SenderUserId { get; set; } = "";
        [JsonPropertyName("senderName")] public string SenderName { get; set; } = "";
        [JsonPropertyName("senderRole")] public string SenderRole { get; set; } = "";
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("message")] public string Message { get; set; } = "";
        [JsonPropertyName("type")] public string Type { get; set; } = "message";
        [JsonPropertyName("isRead")] public bool IsRead { get; set; }
        [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }
    }
}
