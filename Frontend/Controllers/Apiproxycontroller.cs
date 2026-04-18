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

        public ApiProxyController(ApiService api) { _api = api; }

        private async Task<string> Body()
        {
            using var r = new System.IO.StreamReader(Request.Body);
            return await r.ReadToEndAsync();
        }

        private IActionResult Fwd(bool ok, string body, int status)
        {
            Response.ContentType = "application/json";
            return ok ? Content(body, "application/json") : StatusCode(status, body);
        }

        // ── Users ──────────────────────────────────────────────
        [HttpGet("api/User")] public async Task<IActionResult> GetUsers() { var (ok, b, s) = await _api.GetAsync("/api/User"); return Fwd(ok, b, s); }
        [HttpGet("api/User/{id:int}")] public async Task<IActionResult> GetUser(int id) { var (ok, b, s) = await _api.GetAsync($"/api/User/{id}"); return Fwd(ok, b, s); }
        [HttpPost("api/User")] public async Task<IActionResult> PostUser() { var j = await Body(); var (ok, b, s) = await _api.PostAsync("/api/User", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpPut("api/User/{id:int}")] public async Task<IActionResult> PutUser(int id) { var j = await Body(); var (ok, b, s) = await _api.PutAsync($"/api/User/{id}", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpDelete("api/User/{id:int}")] public async Task<IActionResult> DelUser(int id) { var (ok, b, s) = await _api.DeleteAsync($"/api/User/{id}"); return Fwd(ok, b, s); }

        // ── Students ───────────────────────────────────────────
        [HttpGet("api/Student")] public async Task<IActionResult> GetStudents() { var (ok, b, s) = await _api.GetAsync("/api/Student"); return Fwd(ok, b, s); }
        [HttpGet("api/Student/{id:int}")] public async Task<IActionResult> GetStudent(int id) { var (ok, b, s) = await _api.GetAsync($"/api/Student/{id}"); return Fwd(ok, b, s); }
        [HttpPost("api/Student")] public async Task<IActionResult> PostStudent() { var j = await Body(); var (ok, b, s) = await _api.PostAsync("/api/Student", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpPut("api/Student/{id:int}")] public async Task<IActionResult> PutStudent(int id) { var j = await Body(); var (ok, b, s) = await _api.PutAsync($"/api/Student/{id}", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpDelete("api/Student/{id:int}")] public async Task<IActionResult> DelStudent(int id) { var (ok, b, s) = await _api.DeleteAsync($"/api/Student/{id}"); return Fwd(ok, b, s); }

        // ── Teachers ───────────────────────────────────────────
        [HttpGet("api/Teacher")] public async Task<IActionResult> GetTeachers() { var (ok, b, s) = await _api.GetAsync("/api/Teacher"); return Fwd(ok, b, s); }
        [HttpGet("api/Teacher/{id:int}")] public async Task<IActionResult> GetTeacher(int id) { var (ok, b, s) = await _api.GetAsync($"/api/Teacher/{id}"); return Fwd(ok, b, s); }
        [HttpPost("api/Teacher")] public async Task<IActionResult> PostTeacher() { var j = await Body(); var (ok, b, s) = await _api.PostAsync("/api/Teacher", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpPut("api/Teacher/{id:int}")] public async Task<IActionResult> PutTeacher(int id) { var j = await Body(); var (ok, b, s) = await _api.PutAsync($"/api/Teacher/{id}", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpDelete("api/Teacher/{id:int}")] public async Task<IActionResult> DelTeacher(int id) { var (ok, b, s) = await _api.DeleteAsync($"/api/Teacher/{id}"); return Fwd(ok, b, s); }

        // ── Attendance ─────────────────────────────────────────
        [HttpGet("AttendanceManagement/Attendance")] public async Task<IActionResult> GetAtts() { var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Attendance"); return Fwd(ok, b, s); }
        [HttpGet("AttendanceManagement/Attendance/{id:int}")] public async Task<IActionResult> GetAtt(int id) { var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Attendance/{id}"); return Fwd(ok, b, s); }
        [HttpPost("AttendanceManagement/Attendance")] public async Task<IActionResult> PostAtt() { var j = await Body(); var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Attendance", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpPut("AttendanceManagement/Attendance/{id:int}")] public async Task<IActionResult> PutAtt(int id) { var j = await Body(); var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Attendance/{id}", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpDelete("AttendanceManagement/Attendance/{id:int}")] public async Task<IActionResult> DelAtt(int id) { var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Attendance/{id}"); return Fwd(ok, b, s); }

        // ── Courses (no delete) ────────────────────────────────
        [HttpGet("AttendanceManagement/Course")] public async Task<IActionResult> GetCourses() { var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Course"); return Fwd(ok, b, s); }
        [HttpGet("AttendanceManagement/Course/{id:int}")] public async Task<IActionResult> GetCourse(int id) { var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Course/{id}"); return Fwd(ok, b, s); }
        [HttpPost("AttendanceManagement/Course")] public async Task<IActionResult> PostCourse() { var j = await Body(); var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Course", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpPut("AttendanceManagement/Course/{id:int}")] public async Task<IActionResult> PutCourse(int id) { var j = await Body(); var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Course/{id}", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpDelete("AttendanceManagement/Course/{id:int}")] public IActionResult DelCourse(int id) { return StatusCode(403, "{\"message\":\"Courses cannot be deleted.\"}"); }

        // ── Enrollment ─────────────────────────────────────────
        [HttpGet("AttendanceManagement/Enrollment")] public async Task<IActionResult> GetEnrolls() { var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Enrollment"); return Fwd(ok, b, s); }
        [HttpGet("AttendanceManagement/Enrollment/{id:int}")] public async Task<IActionResult> GetEnroll(int id) { var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Enrollment/{id}"); return Fwd(ok, b, s); }
        [HttpPost("AttendanceManagement/Enrollment")] public async Task<IActionResult> PostEnroll() { var j = await Body(); var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Enrollment", JsonSerializer.Deserialize<object>(j)!); return Fwd(ok, b, s); }
        [HttpDelete("AttendanceManagement/Enrollment/{id:int}")] public async Task<IActionResult> DelEnroll(int id) { var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Enrollment/{id}"); return Fwd(ok, b, s); }

        // ── Other lookups ──────────────────────────────────────
        [HttpGet("AttendanceManagement/Schedule")] public async Task<IActionResult> GetSchedules() { var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Schedule"); return Fwd(ok, b, s); }
        [HttpGet("AttendanceManagement/Department")] public async Task<IActionResult> GetDepts() { var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Department"); return Fwd(ok, b, s); }
        [HttpGet("AttendanceManagement/Program")] public async Task<IActionResult> GetPrograms() { var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Program"); return Fwd(ok, b, s); }

        private static readonly List<MsgItem> _msgs = new();
        private static int _seq = 1;

        [HttpGet("api/Notifications")]
        public IActionResult GetNotifications()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            var userId = HttpContext.Session.GetString("UserId") ?? "";

            List<MsgItem> result;
            if (role == "admin")
                result = _msgs.Where(m => m.RecipientId == "admin" || m.RecipientId == "all").ToList();
            else if (role == "teacher")
                result = _msgs.Where(m => m.RecipientId == userId || m.RecipientId == "teacher" || m.RecipientId == "all").ToList();
            else
                result = _msgs.Where(m => m.RecipientId == userId || m.RecipientId == "all").ToList();

            return Ok(result.OrderByDescending(m => m.CreatedAt).Take(30));
        }

        [HttpPost("api/Notifications")]
        public IActionResult PostNotification([FromBody] MsgItem dto)
        {
            dto.Id = _seq++;
            dto.CreatedAt = DateTime.UtcNow;
            dto.IsRead = false;
            _msgs.Add(dto);
            return Ok(dto);
        }

        [HttpPut("api/Notifications/{id:int}/read")]
        public IActionResult MarkRead(int id)
        {
            var m = _msgs.FirstOrDefault(x => x.Id == id);
            if (m != null) m.IsRead = true;
            return Ok();
        }

        [HttpDelete("api/Notifications")]
        public IActionResult ClearNotifications()
        {
            var role = HttpContext.Session.GetString("UserRole") ?? "";
            var userId = HttpContext.Session.GetString("UserId") ?? "";
            if (role == "admin") _msgs.RemoveAll(m => m.RecipientId == "admin" || m.RecipientId == "all");
            else _msgs.RemoveAll(m => m.RecipientId == userId);
            return Ok();
        }
    }

    public class MsgItem
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("recipientId")] public string RecipientId { get; set; } = "admin";
        [JsonPropertyName("senderName")] public string SenderName { get; set; } = "";
        [JsonPropertyName("senderRole")] public string SenderRole { get; set; } = "";
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("message")] public string Message { get; set; } = "";
        [JsonPropertyName("type")] public string Type { get; set; } = "message";
        [JsonPropertyName("isRead")] public bool IsRead { get; set; }
        [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }
    }
}