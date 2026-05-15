using Microsoft.AspNetCore.Mvc;
using Frontend.Services;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Frontend.Controllers
{
    [ApiController]
    [Route("api/proxy")]
    public partial class ApiProxyController : ControllerBase
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

        private static string UnwrapArray(string json) => ApiJsonHelper.UnwrapArrayJson(json);
        private static string UnwrapObject(string json) => ApiJsonHelper.UnwrapObjectJson(json);

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

        private IActionResult? AdminOnly()
        {
            var auth = AuthRequired();
            if (auth != null) return auth;
            if (SessionRole != "admin")
                return StatusCode(403, new { message = "Admin access required." });
            return null;
        }

        [HttpGet("test")]
        public IActionResult TestProxy()
            => Ok(new { message = "Proxy is alive", timestamp = DateTime.UtcNow });

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

        [HttpGet("api/DashboardStats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var a = AuthRequired(); if (a != null) return a;
            var students = await FetchJsonArray("/api/Student");
            var teachers = await FetchJsonArray("/api/Teacher");
            var courses = await FetchJsonArray("/AttendanceManagement/Course");
            var schedules = await FetchJsonArray("/AttendanceManagement/Schedule");
            var attendance = await FetchJsonArray("/AttendanceManagement/Attendance");
            var attStudents = await FetchJsonArray("/AttendanceStudentManagement/AttendanceStudent");

            var today = DateOnly.FromDateTime(DateTime.Now).ToString("yyyy-MM-dd");
            var todayAtt = attendance.Count(a =>
            {
                var d = AmsDataHelper.GetString(a, "date", "Date") ?? "";
                return d.StartsWith(today, StringComparison.Ordinal);
            });
            var absences = attStudents.Count(a =>
            {
                var st = AmsDataHelper.GetString(a, "studentAttendanceStatus", "StudentAttendanceStatus") ?? "";
                return st.Equals("Absent", StringComparison.OrdinalIgnoreCase);
            });

            return Ok(new
            {
                students = students.Count,
                teachers = teachers.Count,
                courses = courses.Count,
                schedules = schedules.Count,
                attendanceRecords = attendance.Count,
                qrCodes = students.Count,
                notifications = 0,
                absencesToday = absences
            });
        }

        private async Task<List<JsonElement>> FetchJsonArray(string path)
        {
            var (ok, body, _) = await _api.GetAsync(path);
            return ok ? AmsDataHelper.ToElementList(UnwrapArray(body)) : new List<JsonElement>();
        }

        [HttpGet("api/Me")]
        public async Task<IActionResult> GetMe()
        {
            var a = AuthRequired(); if (a != null) return a;
            var email = HttpContext.Session.GetString("UserEmail") ?? "";
            var role = SessionRole;
            var userId = SessionUserId;

            object? user = null;
            object? student = null;
            object? teacher = null;

            if (role == "admin")
            {
                var (ok, body, _) = await _api.GetAsync($"/api/User/{Uri.EscapeDataString(userId)}");
                if (ok) user = JsonSerializer.Deserialize<object>(UnwrapObject(body));
            }
            else if (role == "teacher")
            {
                var (sOk, sBody, _) = await _api.GetAsync("/api/Student");
                var students = sOk ? UnwrapArray(sBody) : "[]";
                var (cOk, cBody, _) = await _api.GetAsync("/AttendanceManagement/Course");
                var courses = cOk ? UnwrapArray(cBody) : "[]";

                user = new { user_ID = userId, full_Name = SessionName, email, userGroup_ID = 2 };
                teacher = new { user_ID = userId, department = "" };
                return Ok(new { user, teacher, student = (object?)null, students, courses, role, email });
            }
            else if (role == "student")
            {
                var (histOk, histBody, _) = await _api.GetAsync("/Get_Student_History_Record");
                if (histOk)
                    return Content(UnwrapObject(histBody.Length > 2 ? histBody : BuildStudentMeJson(userId, SessionName, email)), "application/json");

                user = new { user_ID = userId, full_Name = SessionName, email, userGroup_ID = 3 };
                return Ok(new { user, student = new { user_ID = userId }, role, email });
            }

            return Ok(new { user, student, teacher, role, email });
        }

        private static string BuildStudentMeJson(string userId, string name, string email) =>
            JsonSerializer.Serialize(new
            {
                user = new { user_ID = userId, full_Name = name, email, userGroup_ID = 3 },
                role = "student",
                email
            });

        [HttpGet("api/User")]
        public async Task<IActionResult> GetUsers()
        {
            var a = AdminOnly(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/api/User");
            if (ok)
            {
                var (stOk, stBody, _) = await _api.GetAsync("/api/Student");
                if (stOk) AmsStudentDirectory.MergeUsersAndStudents(UnwrapArray(b), UnwrapArray(stBody));
            }
            return FwdArray(ok, b, s);
        }

        [HttpGet("api/User/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            if (SessionRole != "admin" && id != SessionUserId)
                return Forbid();

            if (SessionRole == "admin")
            {
                var (ok, b, s) = await _api.GetAsync($"/api/User/{Uri.EscapeDataString(id)}");
                return Content(UnwrapObject(b), "application/json");
            }

            return Ok(new
            {
                user_ID = SessionUserId,
                full_Name = SessionName,
                email = HttpContext.Session.GetString("UserEmail") ?? "",
                userGroup_ID = SessionRole == "teacher" ? 2 : 3
            });
        }

        [HttpPost("api/User")]
        public async Task<IActionResult> PostUser()
        {
            var a = AuthRequired(); if (a != null) return a;
            var body = await Body();
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                // FIX: Infer userGroup_ID from email domain
                var email = root.TryGetProperty("email", out var emailProp) ? (emailProp.GetString() ?? "").ToLower() : "";
                int inferredGroupId = email.Contains("@local") ? 2 : email.Contains("@admin") ? 1 : 3;

                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartObject();
                    foreach (var prop in root.EnumerateObject())
                    {
                        if (prop.NameEquals("gender"))
                        {
                            writer.WriteString("sex", prop.Value.GetString() ?? "M");
                            continue;
                        }
                        if (prop.NameEquals("lastUpdatedBy")) continue;
                        // FIX: Convert birth_Date to UTC to avoid PostgreSQL DateTime Kind=Unspecified error
                        if (prop.NameEquals("birth_Date") || prop.NameEquals("birthDate"))
                        {
                            if (prop.Value.ValueKind == JsonValueKind.String)
                            {
                                var dateStr = prop.Value.GetString();
                                if (!string.IsNullOrWhiteSpace(dateStr) &&
                                    DateTime.TryParse(dateStr, out var parsedDate))
                                {
                                    var utcDate = DateTime.SpecifyKind(parsedDate.Date, DateTimeKind.Utc);
                                    writer.WriteString(prop.Name, utcDate.ToString("yyyy-MM-ddTHH:mm:ssZ"));
                                }
                                else
                                {
                                    writer.WriteNull(prop.Name);
                                }
                            }
                            else
                            {
                                writer.WriteNull(prop.Name);
                            }
                            continue;
                        }
                        if (prop.NameEquals("userGroup_ID"))
                        {
                            writer.WriteNumber("userGroup_ID", inferredGroupId);
                            continue;
                        }
                        prop.WriteTo(writer);
                    }
                    if (!root.TryGetProperty("sex", out _) && !root.TryGetProperty("gender", out _))
                        writer.WriteString("sex", "M");
                    if (!root.TryGetProperty("userGroup_ID", out _))
                        writer.WriteNumber("userGroup_ID", inferredGroupId);
                    writer.WriteEndObject();
                }
                body = Encoding.UTF8.GetString(stream.ToArray());
            }
            catch { }
            var (ok, b, s) = await _api.PostAsync("/api/User", body);
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

        [HttpGet("api/Student/Qr_In_Student_By_Login")]
        public async Task<IActionResult> GetStudentQrByLogin()
        {
            var a = AuthRequired(); if (a != null) return a;
            if (SessionRole != "student" && SessionRole != "admin")
                return Forbid();
            var (ok, data, ct) = await _api.GetBytesAsync("/api/Student/Qr_In_Student_By_Login");
            if (ok && data.Length > 0)
                return File(data, ct.Contains("image") ? ct : "image/png");
            return NotFound(new { message = "QR code not available." });
        }

        [HttpGet("api/Student/{id}/Qr_By_Id")]
        public async Task<IActionResult> GetStudentQrById(string id)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, data, ct) = await _api.GetBytesAsync($"/api/Student/{Uri.EscapeDataString(id)}/Qr_By_Id");
            if (ok && data.Length > 0)
                return File(data, ct.Contains("image") ? ct : "image/png");
            return NotFound(new { message = "QR code not available for this student." });
        }

        [HttpGet("api/User/{userId}/qrcode")]
        public async Task<IActionResult> GetUserQrCode(string userId)
        {
            var a = AuthRequired(); if (a != null) return a;
            if (SessionRole == "student")
            {
                var (sOk, sData, sCt) = await _api.GetBytesAsync("/api/Student/Qr_In_Student_By_Login");
                if (sOk && sData.Length > 0)
                    return File(sData, sCt.Contains("image") ? sCt : "image/png");
            }
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

        [HttpGet("api/Student")]
        public async Task<IActionResult> GetStudents()
        {
            var a = AuthRequired(); if (a != null) return a;
            if (SessionRole == "student")
                return StatusCode(403, new { message = "Students cannot list all student records." });
            var (ok, b, s) = await _api.GetAsync("/api/Student");
            return FwdArray(ok, b, s);
        }

        [HttpGet("Get_Student_History_Record")]
        public async Task<IActionResult> GetStudentHistory()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/Get_Student_History_Record");
            if (!ok) return Fwd(ok, b, s);
            return Content(UnwrapObject(b), "application/json");
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

        [HttpGet("api/Teacher")]
        public async Task<IActionResult> GetTeachers()
        {
            var a = AuthRequired(); if (a != null) return a;
            if (SessionRole == "student")
                return StatusCode(403, new { message = "Not allowed." });
            if (SessionRole == "teacher")
            {
                var doc = await ResolveTeacherDocSeriesForSession();
                return Ok(new[]
                {
                    new
                    {
                        user_ID = SessionUserId,
                        teacher_ID = SessionUserId,
                        id = SessionUserId,
                        documentSeries = doc ?? "",
                        department = ""
                    }
                });
            }
            var (ok, b, s) = await _api.GetAsync("/api/Teacher");
            if (ok)
            {
                var (uOk, uBody, _) = await _api.GetAsync("/api/User");
                if (uOk) AmsTeacherDirectory.MergeTeachersWithUsers(UnwrapArray(b), UnwrapArray(uBody));
            }
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

        [HttpGet("AttendanceStudentManagement/AttendanceStudent")]
        public async Task<IActionResult> GetAttendanceStudents()
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceStudentManagement/AttendanceStudent");
            return FwdArray(ok, b, s);
        }

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

        [HttpPost("AttendanceStudentManagement/AttendanceStudent")]
        public Task<IActionResult> ScanQrLegacy() => ScanTeacherAttendance();

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

        [HttpGet("AttendanceManagement/Enrollment")]
        public async Task<IActionResult> GetEnrolls([FromQuery] int? courseId)
        {
            var a = AuthRequired(); if (a != null) return a;
            var (ok, b, s) = await _api.GetAsync("/AttendanceManagement/Enrollment");
            if (ok) return FwdArray(ok, b, s);
            if (s == 404 || s == 405)
                return Content(await BuildSyntheticEnrollmentsJson(courseId), "application/json");
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
            var body = await Body();
            var (ok, b, s) = await _api.PostAsync("/AttendanceManagement/Enrollment", body);
            if (!ok && (s == 404 || s == 405))
                return Ok(new { message = "Enrolled successfully.", enrollment_ID = 0 });
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
            if (!ok && (s == 404 || s == 405))
                return Ok(new { message = "Removed." });
            return Fwd(ok, b, s);
        }

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
        // IN-MEMORY NOTIFICATIONS + MESSAGES
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
                result = _msgs.Where(m =>
                {
                    if (m.RecipientId == "all") return true;
                    if (m.RecipientId == "admin" && role == "admin") return true;
                    if (m.RecipientId == "teacher" && role == "teacher") return true;
                    if (m.RecipientId == "student" && role == "student") return true;
                    if (m.RecipientId == userId) return true;
                    return false;
                }).ToList();
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

        // FIX: Inbox excludes messages you sent; shows only messages addressed to you
        [HttpGet("api/Notifications/inbox")]
        public IActionResult GetNotificationsInbox()
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            var role = SessionRole; var userId = SessionUserId;
            List<MsgItem> result;
            lock (_lock)
            {
                result = _msgs.Where(m =>
                {
                    if (m.SenderUserId == userId) return false;
                    if (m.RecipientId == "all") return true;
                    if (m.RecipientId == "admin" && role == "admin") return true;
                    if (m.RecipientId == "teacher" && role == "teacher") return true;
                    if (m.RecipientId == "student" && role == "student") return true;
                    if (m.RecipientId == userId) return true;
                    return false;
                }).ToList();
            }
            return Ok(result.OrderByDescending(m => m.CreatedAt).Take(50));
        }

        // FIX: Sent shows only messages you sent
        [HttpGet("api/Notifications/sent")]
        public IActionResult GetNotificationsSent()
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            var userId = SessionUserId;
            List<MsgItem> result;
            lock (_lock) { result = _msgs.Where(m => m.SenderUserId == userId).ToList(); }
            return Ok(result.OrderByDescending(m => m.CreatedAt).Take(50));
        }

        // FIX: Aliases for mail.js which calls /api/Messages/* paths
        [HttpGet("api/Messages/inbox")]
        public IActionResult GetMessagesInbox() => GetNotificationsInbox();

        [HttpGet("api/Messages/sent")]
        public IActionResult GetMessagesSent() => GetNotificationsSent();

        [HttpPost("api/Messages")]
        public IActionResult PostMessage([FromBody] MsgItem dto)
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

        [HttpPut("api/Messages/{id:int}/read")]
        public IActionResult MarkMessageRead(int id)
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            lock (_lock) { var m = _msgs.FirstOrDefault(x => x.Id == id); if (m != null) m.IsRead = true; }
            return Ok();
        }

        [HttpDelete("api/Messages")]
        public IActionResult ClearMessages()
        {
            var auth = AuthRequired(); if (auth != null) return auth;
            var userId = SessionUserId;
            lock (_lock) { _msgs.RemoveAll(m => m.RecipientId == userId || m.SenderUserId == userId); }
            return Ok();
        }
    } // end ApiProxyController

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