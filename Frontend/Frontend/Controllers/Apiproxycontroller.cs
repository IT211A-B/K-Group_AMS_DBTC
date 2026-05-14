using Microsoft.AspNetCore.Mvc;
using Frontend.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Frontend.Controllers
{
    [ApiController]
    [Route("api/proxy")]
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

        // Unwraps {"$id":"1","$values":[...]} → returns the raw JSON as-is.
        // The JS safeArr() helper handles $values on the client side.
        private static string UnwrapArray(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Object &&
                    root.TryGetProperty("$values", out var values))
                    return values.GetRawText();
                return json;
            }
            catch { return json; }
        }

        private IActionResult Fwd(bool ok, string body, int status)
        {
            if (!ok && status == 401)
                return Unauthorized(new { message = "Session expired", redirect = "/Login/Index" });
            return StatusCode(status, body.Length > 0 ? body : "{}");
        }

        private IActionResult FwdArray(bool ok, string body, int status)
        {
            if (!ok) return Fwd(ok, body, status);
            return Content(UnwrapArray(body), "application/json");
        }

        private IActionResult? AuthRequired()
        {
            if (!IsAuthenticated)
                return Unauthorized(new { message = "Not authenticated. Please log in." });
            return null;
        }

        // ── Health check ────────────────────────────────────────────────────────
        [HttpGet("test")]
        public IActionResult TestProxy()
            => Ok(new { message = "Proxy is alive", timestamp = DateTime.UtcNow });

        // ── Session info ────────────────────────────────────────────────────────
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

        // ══════════════════════════════════════════════════════════════════════
        // USER
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("api/User")]
        public async Task<IActionResult> GetUsers()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/api/User");
            return FwdArray(ok, b, s);
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
            var (ok, b, s) = await _api.PostAsync("/api/User", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("api/User/{id}")]
        public async Task<IActionResult> PutUser(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/api/User/{Uri.EscapeDataString(id)}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("api/User/{id}")]
        public async Task<IActionResult> DelUser(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/api/User/{Uri.EscapeDataString(id)}");
            return Fwd(ok, b, s);
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

        // ── User search helper ─────────────────────────────────────────────────
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
                var qLow = q.ToLower();
                var users = root.EnumerateArray()
                    .Where(u =>
                    {
                        var name = u.TryGetProperty("full_Name", out var fn) ? fn.GetString() ?? "" : "";
                        var email = u.TryGetProperty("email", out var em) ? em.GetString() ?? "" : "";
                        return name.ToLower().Contains(qLow) || email.ToLower().Contains(qLow);
                    })
                    .Select(u => new
                    {
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

        // ══════════════════════════════════════════════════════════════════════
        // STUDENT
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("api/Student")]
        public async Task<IActionResult> GetStudents()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/api/Student");
            return FwdArray(ok, b, s);
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
            var (ok, b, s) = await _api.PostAsync("/api/Student", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("api/Student/{id}")]
        public async Task<IActionResult> PutStudent(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/api/Student/{Uri.EscapeDataString(id)}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("api/Student/{id}")]
        public async Task<IActionResult> DelStudent(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/api/Student/{Uri.EscapeDataString(id)}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // TEACHER
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("api/Teacher")]
        public async Task<IActionResult> GetTeachers()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/api/Teacher");
            return FwdArray(ok, b, s);
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
            var (ok, b, s) = await _api.PostAsync("/api/Teacher", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("api/Teacher/{id}")]
        public async Task<IActionResult> PutTeacher(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/api/Teacher/{Uri.EscapeDataString(id)}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("api/Teacher/{id}")]
        public async Task<IActionResult> DelTeacher(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/api/Teacher/{Uri.EscapeDataString(id)}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // ATTENDANCE
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("AttendanceManagement/Attendance")]
        public async Task<IActionResult> GetAtts()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Attendance");
            return FwdArray(ok, b, s);
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
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Attendance", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Attendance/{id:int}")]
        public async Task<IActionResult> PutAtt(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Attendance/{id}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Attendance/{id:int}")]
        public async Task<IActionResult> DelAtt(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Attendance/{id}");
            return Fwd(ok, b, s);
        }

        // ── QR scan endpoint (teacher scans student QR) ────────────────────────
        [HttpPost("AttendanceManagement/Attendance/scan")]
        public async Task<IActionResult> ScanQr()
        {
            var a = AuthRequired(); if (a != null) return a;
            var body = await Body();

            // Parse the scanned userId + scheduleId, compute status, then POST attendance
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                var scannedUserId = root.TryGetProperty("scannedUserId", out var su) ? su.GetString() ?? "" : "";
                var scheduleId = root.TryGetProperty("scheduleId", out var sch) ? sch.GetInt32() : 0;
                var courseId = root.TryGetProperty("courseId", out var ci) ? ci.GetInt32() : 0;
                var scanTimeStr = root.TryGetProperty("scanTime", out var st) ? st.GetString() ?? DateTime.UtcNow.ToString("o") : DateTime.UtcNow.ToString("o");
                var classStartStr = root.TryGetProperty("classStartTime", out var cs) ? cs.GetString() ?? "" : "";

                // Compute status
                string status = "Absent";
                if (!string.IsNullOrEmpty(scanTimeStr) && !string.IsNullOrEmpty(classStartStr))
                {
                    if (DateTime.TryParse(scanTimeStr, out var scan) && DateTime.TryParse(classStartStr, out var classStart))
                    {
                        var diffMins = (scan - classStart).TotalMinutes;
                        if (diffMins >= -30 && diffMins <= 15)
                            status = "Present";
                        else if (diffMins > 15 && diffMins <= 30)
                            status = "Late";
                        else
                            status = "Absent";
                    }
                }

                // Find student_ID from user_ID
                var (sOk, sBody, _) = await _api.GetAsync($"/api/Student");
                int studentId = 0;
                if (sOk)
                {
                    var arr = UnwrapArray(sBody);
                    using var sDoc = JsonDocument.Parse(arr);
                    if (sDoc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var s in sDoc.RootElement.EnumerateArray())
                        {
                            var uid = s.TryGetProperty("user_ID", out var up) ? up.GetString() ?? "" : "";
                            if (uid == scannedUserId)
                            {
                                studentId = s.TryGetProperty("student_ID", out var sp) ? sp.GetInt32() : 0;
                                break;
                            }
                        }
                    }
                }

                var attendancePayload = JsonSerializer.Serialize(new
                {
                    student_ID = studentId,
                    course_ID = courseId,
                    schedule_ID = scheduleId,
                    date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    status = status,
                    scanTime = scanTimeStr,
                    lastUpdatedBy = SessionName
                });

                var (ok, b, sc) = await _api.PostAsync("/AttendanceManagement/Attendance", attendancePayload);
                return Fwd(ok, b, sc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Scan processing error: " + ex.Message });
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        // COURSE
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("AttendanceManagement/Course")]
        public async Task<IActionResult> GetCourses()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Course");
            return FwdArray(ok, b, s);
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
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Course", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Course/{id:int}")]
        public async Task<IActionResult> PutCourse(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Course/{id}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Course/{id:int}")]
        public async Task<IActionResult> DelCourse(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Course/{id}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // ENROLLMENT
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("AttendanceManagement/Enrollment")]
        public async Task<IActionResult> GetEnrolls()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Enrollment");
            return FwdArray(ok, b, s);
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
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Enrollment", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Enrollment/{id:int}")]
        public async Task<IActionResult> PutEnroll(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Enrollment/{id}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Enrollment/{id:int}")]
        public async Task<IActionResult> DelEnroll(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Enrollment/{id}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // DEPARTMENT
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("AttendanceManagement/Department")]
        public async Task<IActionResult> GetDepts()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Department");
            return FwdArray(ok, b, s);
        }

        [HttpGet("AttendanceManagement/Department/{id:int}")]
        public async Task<IActionResult> GetDept(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Department/{id}");
            return Fwd(ok, b, s);
        }

        [HttpPost("AttendanceManagement/Department")]
        public async Task<IActionResult> PostDept()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Department", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Department/{id:int}")]
        public async Task<IActionResult> PutDept(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Department/{id}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Department/{id:int}")]
        public async Task<IActionResult> DelDept(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Department/{id}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // PROGRAM
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("AttendanceManagement/Program")]
        public async Task<IActionResult> GetPrograms()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Program");
            return FwdArray(ok, b, s);
        }

        [HttpGet("AttendanceManagement/Program/{id:int}")]
        public async Task<IActionResult> GetProgram(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Program/{id}");
            return Fwd(ok, b, s);
        }

        [HttpPost("AttendanceManagement/Program")]
        public async Task<IActionResult> PostProgram()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Program", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Program/{id:int}")]
        public async Task<IActionResult> PutProgram(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Program/{id}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Program/{id:int}")]
        public async Task<IActionResult> DelProgram(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Program/{id}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // SCHEDULE
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("AttendanceManagement/Schedule")]
        public async Task<IActionResult> GetSchedules()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Schedule");
            return FwdArray(ok, b, s);
        }

        [HttpGet("AttendanceManagement/Schedule/{id:int}")]
        public async Task<IActionResult> GetSchedule(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Schedule/{id}");
            return Fwd(ok, b, s);
        }

        [HttpPost("AttendanceManagement/Schedule")]
        public async Task<IActionResult> PostSchedule()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Schedule", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Schedule/{id:int}")]
        public async Task<IActionResult> PutSchedule(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Schedule/{id}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Schedule/{id:int}")]
        public async Task<IActionResult> DelSchedule(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Schedule/{id}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // SECTION
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("AttendanceManagement/Section")]
        public async Task<IActionResult> GetSections()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Section");
            return FwdArray(ok, b, s);
        }

        [HttpGet("AttendanceManagement/Section/{id:int}")]
        public async Task<IActionResult> GetSection(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Section/{id}");
            return Fwd(ok, b, s);
        }

        [HttpPost("AttendanceManagement/Section")]
        public async Task<IActionResult> PostSection()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Section", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Section/{id:int}")]
        public async Task<IActionResult> PutSection(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Section/{id}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Section/{id:int}")]
        public async Task<IActionResult> DelSection(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Section/{id}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // PERMISSION
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("AttendanceManagement/Permission")]
        public async Task<IActionResult> GetPerms()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Permission");
            return FwdArray(ok, b, s);
        }

        [HttpGet("AttendanceManagement/Permission/{id:int}")]
        public async Task<IActionResult> GetPerm(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/Permission/{id}");
            return Fwd(ok, b, s);
        }

        [HttpPost("AttendanceManagement/Permission")]
        public async Task<IActionResult> PostPerm()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Permission", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/Permission/{id:int}")]
        public async Task<IActionResult> PutPerm(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/Permission/{id}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/Permission/{id:int}")]
        public async Task<IActionResult> DelPerm(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/Permission/{id}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // ROLE PERMISSION
        // ══════════════════════════════════════════════════════════════════════

        [HttpGet("AttendanceManagement/RolePermission")]
        public async Task<IActionResult> GetRolePerms()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/RolePermission");
            return FwdArray(ok, b, s);
        }

        [HttpGet("AttendanceManagement/RolePermission/{id:int}")]
        public async Task<IActionResult> GetRolePerm(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync($"/AttendanceManagement/RolePermission/{id}");
            return Fwd(ok, b, s);
        }

        [HttpPost("AttendanceManagement/RolePermission")]
        public async Task<IActionResult> PostRolePerm()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/RolePermission", await Body());
            return Fwd(ok, b, s);
        }

        [HttpPut("AttendanceManagement/RolePermission/{id:int}")]
        public async Task<IActionResult> PutRolePerm(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.PutAsync($"/AttendanceManagement/RolePermission/{id}", await Body());
            return Fwd(ok, b, s);
        }

        [HttpDelete("AttendanceManagement/RolePermission/{id:int}")]
        public async Task<IActionResult> DelRolePerm(int id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.DeleteAsync($"/AttendanceManagement/RolePermission/{id}");
            return Fwd(ok, b, s);
        }

        // ══════════════════════════════════════════════════════════════════════
        // IN-MEMORY NOTIFICATIONS (no backend endpoint needed)
        // ══════════════════════════════════════════════════════════════════════

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
                result = role switch
                {
                    "admin" => _msgs.Where(m => m.RecipientId == "admin" || m.RecipientId == "all" || m.RecipientId == userId).ToList(),
                    "teacher" => _msgs.Where(m => m.RecipientId == "teacher" || m.RecipientId == "all" || m.RecipientId == userId).ToList(),
                    _ => _msgs.Where(m => m.RecipientId == "all" || m.RecipientId == userId).ToList()
                };
            }
            return Ok(result.OrderByDescending(m => m.CreatedAt).Take(50));
        }

        [HttpPost("api/Notifications")]
        public IActionResult PostNotification([FromBody] MsgItem dto)
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest(new { message = "Subject is required." });
            if (string.IsNullOrWhiteSpace(dto.Message)) return BadRequest(new { message = "Message body is required." });
            dto.SenderName = SessionName;
            dto.SenderRole = SessionRole;
            dto.SenderUserId = SessionUserId;
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

    // ── DTO ───────────────────────────────────────────────────────────────────
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