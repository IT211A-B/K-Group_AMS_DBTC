using Microsoft.AspNetCore.Mvc;
using Frontend.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Frontend.Controllers
{
    [ApiController]
    public class ApiProxyController : ControllerBase
    {
        private readonly ApiService _api;
        public ApiProxyController(ApiService api) => _api = api;

        private string SessionUserId => HttpContext.Session.GetString("UserId") ?? "";
        private string SessionRole => HttpContext.Session.GetString("UserRole") ?? "";
        private string SessionName => HttpContext.Session.GetString("UserName") ?? "";

        private bool IsAuthenticated => !string.IsNullOrEmpty(SessionRole);

        private async Task<string> Body()
        {
            using var r = new StreamReader(Request.Body);
            return await r.ReadToEndAsync();
        }

        private static string UnwrapArray(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("$values", out var values))
                    return values.GetRawText(); // get raw text BEFORE dispose
                return json;
            }
            catch { return json; }
        }

        private IActionResult Fwd(bool ok, string body, int status)
        {
            if (!ok && status == 401)
                return Unauthorized(new { message = "Session expired", redirect = "/Login/Index" });
            Response.ContentType = "application/json";
            return StatusCode(status, body);
        }

        private IActionResult? AuthRequired()
        {
            if (!IsAuthenticated)
                return Unauthorized(new { message = "Not authenticated. Please log in." });
            return null;
        }

        [HttpGet("api/SessionInfo")]
        public IActionResult GetSessionInfo()
        {
            if (!IsAuthenticated)
                return Unauthorized(new { message = "Not authenticated." });
            return Ok(new
            {
                userId = SessionUserId,
                role = SessionRole,
                name = SessionName,
                email = HttpContext.Session.GetString("UserEmail") ?? ""
            });
        }

        [HttpGet("api/User")]
        public async Task<IActionResult> GetUsers()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/api/User");
            if (!ok) return Fwd(ok, b, s);
            Response.ContentType = "application/json";
            return Content(UnwrapArray(b), "application/json");
        }

        [HttpGet("api/User/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/api/User/{Uri.EscapeDataString(id)}");
            return Fwd(ok, b, s);
        }

        [HttpPost("api/User")]
        public async Task<IActionResult> PostUser()
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PostAsync("/api/User", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpPut("api/User/{id}")]
        public async Task<IActionResult> PutUser(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PutAsync($"/api/User/{Uri.EscapeDataString(id)}", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpDelete("api/User/{id}")]
        public async Task<IActionResult> DelUser(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/api/User/{Uri.EscapeDataString(id)}");
            return Fwd(ok, b, s);
        }

        [HttpGet("api/Student")]
        public async Task<IActionResult> GetStudents()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/api/Student");
            if (!ok) return Fwd(ok, b, s);
            return Content(UnwrapArray(b), "application/json");
        }

        [HttpGet("api/Student/{id}")]
        public async Task<IActionResult> GetStudent(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/api/Student/{Uri.EscapeDataString(id)}");
            return Fwd(ok, b, s);
        }

        [HttpPost("api/Student")]
        public async Task<IActionResult> PostStudent()
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PostAsync("/api/Student", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpPut("api/Student/{id}")]
        public async Task<IActionResult> PutStudent(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PutAsync($"/api/Student/{Uri.EscapeDataString(id)}", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpDelete("api/Student/{id}")]
        public async Task<IActionResult> DelStudent(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/api/Student/{Uri.EscapeDataString(id)}");
            return Fwd(ok, b, s);
        }

        [HttpGet("api/Teacher")]
        public async Task<IActionResult> GetTeachers()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/api/Teacher");
            if (!ok) return Fwd(ok, b, s);
            return Content(UnwrapArray(b), "application/json");
        }

        [HttpGet("api/Teacher/{id}")]
        public async Task<IActionResult> GetTeacher(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/api/Teacher/{Uri.EscapeDataString(id)}");
            return Fwd(ok, b, s);
        }

        [HttpPost("api/Teacher")]
        public async Task<IActionResult> PostTeacher()
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PostAsync("/api/Teacher", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpPut("api/Teacher/{id}")]
        public async Task<IActionResult> PutTeacher(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PutAsync($"/api/Teacher/{Uri.EscapeDataString(id)}", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpDelete("api/Teacher/{id}")]
        public async Task<IActionResult> DelTeacher(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/api/Teacher/{Uri.EscapeDataString(id)}");
            return Fwd(ok, b, s);
        }

        [HttpGet("AttendanceManagement/Attendance")]
        public async Task<IActionResult> GetAtts()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Attendance");
            if (!ok) return Fwd(ok, b, s);
            return Content(UnwrapArray(b), "application/json");
        }

        [HttpGet("AttendanceManagement/Attendance/{id:int}")]
        public async Task<IActionResult> GetAtt(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Attendance/{id}");
            return Fwd(ok, b, s);
        }

        [HttpPost("AttendanceManagement/Attendance")]
        public async Task<IActionResult> PostAtt()
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Attendance", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Attendance/{id:int}")]
        public async Task<IActionResult> PutAtt(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Attendance/{id}", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Attendance/{id:int}")]
        public async Task<IActionResult> DelAtt(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Attendance/{id}");
            return Fwd(ok, b, s);
        }

        [HttpGet("AttendanceManagement/Course")]
        public async Task<IActionResult> GetCourses()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Course");
            if (!ok) return Fwd(ok, b, s);
            return Content(UnwrapArray(b), "application/json");
        }

        [HttpGet("AttendanceManagement/Course/{id:int}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Course/{id}");
            return Fwd(ok, b, s);
        }

        [HttpPost("AttendanceManagement/Course")]
        public async Task<IActionResult> PostCourse()
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Course", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Course/{id:int}")]
        public async Task<IActionResult> PutCourse(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Course/{id}", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Course/{id:int}")]
        public IActionResult DelCourse(int id)
            => StatusCode(403, "{\"message\":\"Courses cannot be deleted.\"}");

        [HttpGet("AttendanceManagement/Enrollment")]
        public async Task<IActionResult> GetEnrolls()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Enrollment");
            if (!ok) return Fwd(ok, b, s);
            return Content(UnwrapArray(b), "application/json");
        }

        [HttpGet("AttendanceManagement/Enrollment/{id:int}")]
        public async Task<IActionResult> GetEnroll(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Enrollment/{id}");
            return Fwd(ok, b, s);
        }

        [HttpPost("AttendanceManagement/Enrollment")]
        public async Task<IActionResult> PostEnroll()
        {
            var a = AuthRequired(); if (a != null) return a;
            var j = await Body();
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Enrollment", JsonSerializer.Deserialize<JsonElement>(j));
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Enrollment/{id:int}")]
        public async Task<IActionResult> DelEnroll(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Enrollment/{id}");
            return Fwd(ok, b, s);
        }

        [HttpGet("AttendanceManagement/Schedule")]
        public async Task<IActionResult> GetSchedules()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Schedule");
            if (!ok) return Fwd(ok, b, s);
            return Content(UnwrapArray(b), "application/json");
        }

        [HttpGet("AttendanceManagement/Department")]
        public async Task<IActionResult> GetDepts()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Department");
            if (!ok) return Fwd(ok, b, s);
            return Content(UnwrapArray(b), "application/json");
        }

        [HttpGet("AttendanceManagement/Program")]
        public async Task<IActionResult> GetPrograms()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Program");
            if (!ok) return Fwd(ok, b, s);
            return Content(UnwrapArray(b), "application/json");
        }

        [HttpGet("api/User/{userId}/qrcode")]
        public async Task<IActionResult> GetUserQrCode(string userId)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, data, ct) = await _api.GetBytesAsync($"/api/User/{Uri.EscapeDataString(userId)}/qrcode");
            if (ok && data.Length > 0)
                return File(data, ct.Contains("image") ? ct : "image/png");
            return NotFound(new { message = "QR code not available for this user." });
        }

        [HttpGet("api/UserSearch")]
        public async Task<IActionResult> SearchUsers([FromQuery] string q)
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2) return Ok(new List<object>());
            var (ok, body, _) = await _api.GetAsync("/api/User");
            if (!ok) return Ok(new List<object>());
            try
            {
                var arrJson = UnwrapArray(body);
                using var doc = JsonDocument.Parse(arrJson);
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Array) return Ok(new List<object>());
                var users = root.EnumerateArray()
                    .Where(u => {
                        var name = u.TryGetProperty("full_Name", out var fn) ? fn.GetString() ?? "" : "";
                        var email = u.TryGetProperty("email", out var em) ? em.GetString() ?? "" : "";
                        var qLow = q.ToLower();
                        return name.ToLower().Contains(qLow) || email.ToLower().Contains(qLow);
                    })
                    .Select(u => new {
                        userId = u.TryGetProperty("user_ID", out var id) ? id.GetString() ?? "" : "",
                        name = u.TryGetProperty("full_Name", out var fn) ? fn.GetString() : "",
                        email = u.TryGetProperty("email", out var em) ? em.GetString() : "",
                        groupId = u.TryGetProperty("userGroup_ID", out var gid) ? gid.ToString() : ""
                    })
                    .Take(10).ToList();
                return Ok(users);
            }
            catch { return Ok(new List<object>()); }
        }

        private static readonly List<MsgItem> _msgs = new();
        private static int _seq = 1;
        private static readonly object _lock = new();

        [HttpGet("api/Notifications")]
        public IActionResult GetNotifications()
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            var role = SessionRole; var userId = SessionUserId;
            List<MsgItem> result;
            lock (_lock)
            {
                if (role == "admin")
                    result = _msgs.Where(m => m.RecipientId == "admin" || m.RecipientId == "all" || m.RecipientId == userId).ToList();
                else if (role == "teacher")
                    result = _msgs.Where(m => m.RecipientId == userId || m.RecipientId == "teacher" || m.RecipientId == "all").ToList();
                else
                    result = _msgs.Where(m => m.RecipientId == userId || m.RecipientId == "all").ToList();
            }
            return Ok(result.OrderByDescending(m => m.CreatedAt).Take(50));
        }

        [HttpPost("api/Notifications")]
        public IActionResult PostNotification([FromBody] MsgItem dto)
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest(new { message = "Subject is required." });
            if (string.IsNullOrWhiteSpace(dto.Message)) return BadRequest(new { message = "Message body is required." });
            dto.SenderName = SessionName; dto.SenderRole = SessionRole; dto.SenderUserId = SessionUserId;
            lock (_lock) { dto.Id = _seq++; dto.CreatedAt = DateTime.UtcNow; dto.IsRead = false; _msgs.Add(dto); }
            return Ok(dto);
        }

        [HttpPut("api/Notifications/{id:int}/read")]
        public IActionResult MarkRead(int id)
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            lock (_lock) { var m = _msgs.FirstOrDefault(x => x.Id == id); if (m != null) m.IsRead = true; }
            return Ok();
        }

        [HttpDelete("api/Notifications")]
        public IActionResult ClearNotifications()
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            var userId = SessionUserId;
            lock (_lock) { _msgs.RemoveAll(m => m.RecipientId == userId || m.RecipientId == "all"); }
            return Ok();
        }

        [HttpDelete("api/Notifications/{id:int}")]
        public IActionResult DeleteNotification(int id)
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            lock (_lock) { _msgs.RemoveAll(m => m.Id == id); }
            return Ok();
        }
    }

    public class MsgItem
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("recipientId")] public string RecipientId { get; set; } = "admin";
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