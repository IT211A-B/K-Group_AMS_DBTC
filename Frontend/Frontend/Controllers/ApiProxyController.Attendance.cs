using System.Text.Json;
using Frontend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
  public partial class ApiProxyController
  {
    private async Task<(bool ok, string body)> FetchJson(string path)
    {
      var (ok, body, _) = await _api.GetAsync(path);
      return (ok, ok ? UnwrapArray(body) : "[]");
    }

    private async Task<string> BuildSyntheticEnrollmentsJson(int? courseId = null)
    {
      var (_, schedulesJson) = await FetchJson("/AttendanceManagement/Schedule");
      var (_, studentsJson) = await FetchJson("/api/Student");
      var (_, coursesJson) = await FetchJson("/AttendanceManagement/Course");

      var schedules = AmsDataHelper.ToElementList(schedulesJson);
      var students = AmsDataHelper.ToElementList(studentsJson);
      var courses = AmsDataHelper.ToElementList(coursesJson);

      var enrollments = new List<object>();
      var enrollmentId = 1;

      foreach (var sch in schedules)
      {
        var schCourseId = AmsDataHelper.GetInt(sch, "course_ID", "Course_ID");
        if (courseId.HasValue && schCourseId != courseId.Value) continue;

        var sectionId = AmsDataHelper.GetInt(sch, "section_ID", "Section_ID");
        var scheduleId = AmsDataHelper.GetInt(sch, "schedule_Id", "Schedule_Id", "schedule_ID", "Schedule_ID");

        foreach (var st in students)
        {
          var stSection = AmsDataHelper.GetInt(st, "sectionID", "SectionID", "section_ID");
          if (stSection != sectionId) continue;

          var studentDocId = AmsDataHelper.ParseDocSeriesNumericId(
            AmsDataHelper.GetString(st, "documentSeries", "DocumentSeries"));
          var studentGuid = AmsDataHelper.GetString(st, "student_ID", "Student_ID", "student_Id");

          enrollments.Add(new
          {
            enrollment_ID = enrollmentId++,
            course_ID = schCourseId,
            schedule_ID = scheduleId,
            section_ID = sectionId,
            student_ID = studentDocId ?? 0,
            student_Guid = studentGuid,
            userDocumentSeries = AmsDataHelper.GetString(st, "userDocumentSeries", "UserDocumentSeries")
          });
        }
      }

      return JsonSerializer.Serialize(enrollments);
    }

    private async Task<string?> ResolveTeacherDocSeriesForSession()
    {
      var email = HttpContext.Session.GetString("UserEmail");
      var cached = AmsTeacherDirectory.ResolveTeacherDocSeries(email);
      if (!string.IsNullOrEmpty(cached)) return cached;

      if (SessionRole != "admin") return null;

      var (tOk, tBody) = await FetchJson("/api/Teacher");
      var (uOk, uBody) = await FetchJson("/api/User");
      if (tOk && uOk) AmsTeacherDirectory.MergeTeachersWithUsers(tBody, uBody);
      return AmsTeacherDirectory.ResolveTeacherDocSeries(email);
    }

    private static IEnumerable<JsonElement> FilterCoursesForTeacher(string coursesJson, string? teacherDocSeries)
    {
      var courses = AmsDataHelper.ToElementList(coursesJson);
      if (string.IsNullOrEmpty(teacherDocSeries)) return courses;
      return courses.Where(c =>
      {
        var tds = AmsDataHelper.GetString(c, "teacherDocumentSeries", "TeacherDocumentSeries", "teacher_ID");
        return string.Equals(tds, teacherDocSeries, StringComparison.OrdinalIgnoreCase);
      });
    }

    [HttpGet("api/teacher/my-courses")]
    public async Task<IActionResult> GetTeacherMyCourses()
    {
      var a = AuthRequired(); if (a != null) return a;
      var (ok, body) = await FetchJson("/AttendanceManagement/Course");
      if (!ok) return Ok(Array.Empty<object>());

      if (SessionRole == "admin")
        return Content(body, "application/json");

      var teacherDoc = await ResolveTeacherDocSeriesForSession();
      var filtered = FilterCoursesForTeacher(body, teacherDoc).ToList();
      var items = new List<JsonElement>();
      foreach (var e in filtered)
      {
        using var doc = JsonDocument.Parse(e.GetRawText());
        items.Add(doc.RootElement.Clone());
      }
      return Content(JsonSerializer.Serialize(items), "application/json");
    }

    [HttpGet("api/teacher/attendance-context")]
    public async Task<IActionResult> GetTeacherAttendanceContext([FromQuery] int? courseId, [FromQuery] int? scheduleId)
    {
      var a = AuthRequired(); if (a != null) return a;
      if (SessionRole != "teacher" && SessionRole != "admin")
        return Forbid();

      var (cOk, coursesJson) = await FetchJson("/AttendanceManagement/Course");
      var (sOk, schedulesJson) = await FetchJson("/AttendanceManagement/Schedule");
      var (stOk, studentsJson) = await FetchJson("/api/Student");
      var (uOk, usersJson) = await FetchJson("/api/User");
      var (aOk, attJson) = await FetchJson("/AttendanceManagement/Attendance");
      var (asOk, attStudJson) = await FetchJson("/AttendanceStudentManagement/AttendanceStudent");

      if (uOk && stOk)
        AmsStudentDirectory.MergeUsersAndStudents(usersJson, studentsJson);

      if (!cOk) coursesJson = "[]";
      if (!sOk) schedulesJson = "[]";
      if (!stOk) studentsJson = "[]";
      if (!aOk) attJson = "[]";
      if (!asOk) attStudJson = "[]";

      var teacherDoc = SessionRole == "admin" ? null : await ResolveTeacherDocSeriesForSession();
      var courses = FilterCoursesForTeacher(coursesJson, teacherDoc).ToList();

      if (courseId.HasValue)
        courses = courses.Where(c => AmsDataHelper.GetInt(c, "course_ID", "Course_ID") == courseId.Value).ToList();

      var schedules = AmsDataHelper.ToElementList(schedulesJson);
      if (courseId.HasValue)
        schedules = schedules.Where(s => AmsDataHelper.GetInt(s, "course_ID", "Course_ID") == courseId.Value).ToList();
      if (scheduleId.HasValue)
        schedules = schedules.Where(s => AmsDataHelper.GetInt(s, "schedule_Id", "Schedule_Id", "schedule_ID") == scheduleId.Value).ToList();

      var today = DateTime.Now;
      var todayDow = today.DayOfWeek;
      var activeSchedules = schedules.Where(s =>
      {
        var dow = AmsDataHelper.ParseDayOfWeek(AmsDataHelper.GetString(s, "dayOfWeek", "DayOfWeek"));
        return dow == null || dow == todayDow;
      }).ToList();

      var enrollJson = await BuildSyntheticEnrollmentsJson(courseId);
      var enrollments = AmsDataHelper.ToElementList(enrollJson);

      return Ok(new
      {
        courses,
        schedules = activeSchedules,
        enrollments,
        students = AmsDataHelper.ToElementList(studentsJson),
        users = uOk ? AmsDataHelper.ToElementList(usersJson) : new List<JsonElement>(),
        attendance = AmsDataHelper.ToElementList(attJson),
        attendanceStudents = AmsDataHelper.ToElementList(attStudJson),
        serverNow = DateTime.Now.ToString("o")
      });
    }

    private async Task<(bool ok, int attendanceId, string? error)> EnsureAttendanceSessionAsync(int scheduleId)
    {
      var (sOk, sBody, _) = await _api.GetAsync($"/AttendanceManagement/Schedule/{scheduleId}");
      if (!sOk) return (false, 0, "Schedule not found.");

      using var schDoc = JsonDocument.Parse(UnwrapObject(sBody));
      var sch = schDoc.RootElement;
      if (sch.TryGetProperty("data", out var schData) || sch.TryGetProperty("Data", out schData)) sch = schData;

      var today = DateTime.Now;
      var dow = AmsDataHelper.ParseDayOfWeek(AmsDataHelper.GetString(sch, "dayOfWeek", "DayOfWeek"));
      if (dow.HasValue && dow.Value != today.DayOfWeek)
        return (false, 0, $"This class is scheduled on {dow}, not {today.DayOfWeek}.");

      if (!AmsDataHelper.TryParseTime(AmsDataHelper.GetString(sch, "startTime", "StartTime"), out var start)
          || !AmsDataHelper.TryParseTime(AmsDataHelper.GetString(sch, "endTime", "EndTime"), out var end))
        return (false, 0, "Schedule times are invalid.");

      var (aOk, aBody) = await FetchJson("/AttendanceManagement/Attendance");
      var todayStr = DateOnly.FromDateTime(today).ToString("yyyy-MM-dd");
      foreach (var att in AmsDataHelper.ToElementList(aOk ? aBody : "[]"))
      {
        var sid = AmsDataHelper.GetInt(att, "schedule_ID", "Schedule_ID");
        var date = AmsDataHelper.GetString(att, "date", "Date") ?? "";
        if (sid == scheduleId && date.StartsWith(todayStr, StringComparison.Ordinal))
          return (true, AmsDataHelper.GetInt(att, "attendance_ID", "Attendance_ID"), null);
      }

      var payload = JsonSerializer.Serialize(new { schedule_ID = scheduleId });
      var (ok, body, _) = await _api.PostAsync("/AttendanceManagement/Attendance", payload);
      if (!ok) return (false, 0, "Could not start attendance session.");

      using var created = JsonDocument.Parse(UnwrapObject(body));
      var data = created.RootElement;
      if (data.TryGetProperty("data", out var inner) || data.TryGetProperty("Data", out inner)) data = inner;
      return (true, AmsDataHelper.GetInt(data, "attendance_ID", "Attendance_ID"), null);
    }

    [HttpPost("api/teacher/start-session")]
    public async Task<IActionResult> StartAttendanceSession()
    {
      var a = AuthRequired(); if (a != null) return a;
      if (SessionRole != "teacher" && SessionRole != "admin")
        return Forbid();

      using var doc = JsonDocument.Parse(await Body());
      var scheduleId = AmsDataHelper.GetInt(doc.RootElement, "schedule_ID", "scheduleId", "Schedule_ID");
      if (scheduleId <= 0) return BadRequest(new { message = "schedule_ID is required." });

      var (ok, attId, err) = await EnsureAttendanceSessionAsync(scheduleId);
      if (!ok) return BadRequest(new { message = err });
      return Ok(new { attendance_ID = attId, schedule_ID = scheduleId });
    }

        /// <summary>
        /// Endpoint used by teachers/admins to scan a student's QR code
        /// and record attendance.
        /// </summary>
        [HttpPost("api/teacher/scan-attendance")] // Creates POST route: /api/teacher/scan-attendance
        public async Task<IActionResult> ScanTeacherAttendance()
        {
            // Check if the user is authenticated/logged in
            // AuthRequired() probably returns:
            // - null if authenticated
            // - IActionResult if unauthorized
            var a = AuthRequired();

            // If authentication failed, immediately return the response
            if (a != null) return a;

            // Check if current session role is NOT teacher and NOT admin
            // Prevents unauthorized users from scanning attendance
            if (SessionRole != "teacher" && SessionRole != "admin")
                return Forbid(); // Returns HTTP 403 Forbidden

            // Read raw request body and parse JSON
            // Example request body:
            // {
            //   "schedule_ID": 1,
            //   "attendance_ID": 2,
            //   "qrToken": "12345"
            // }
            using var doc = JsonDocument.Parse(await Body());

            // RootElement represents the root JSON object
            var root = doc.RootElement;

            // Extract schedule ID from JSON
            // Supports both:
            // - schedule_ID
            // - scheduleId
            var scheduleId = AmsDataHelper.GetInt(root, "schedule_ID", "scheduleId");

            // Extract QR token or scanned user identifier
            // Supports many possible field names for flexibility
            var qrToken = AmsDataHelper.GetString(
                root,
                "QrToken");

            // Validate QR token
            // Prevent empty or whitespace-only QR values
            if (string.IsNullOrWhiteSpace(qrToken))
                return BadRequest(new { message = "Invalid QR code." });

            // Save attendance record through API
            var ok =
                await _api.PostAsync(
                    "/AttendanceStudentManagement/AttendanceStudent",
                    qrToken);

           return Ok(ok);
        }

        [HttpPost("api/admin/assign-student-section")]
    public async Task<IActionResult> AssignStudentSection()
    {
      var a = AdminOnly(); if (a != null) return a;
      using var doc = JsonDocument.Parse(await Body());
      var studentId = AmsDataHelper.GetInt(doc.RootElement, "student_ID", "studentId");
      var sectionId = AmsDataHelper.GetInt(doc.RootElement, "section_ID", "sectionId", "sectionID");
      if (studentId <= 0 || sectionId <= 0)
        return BadRequest(new { message = "student_ID and section_ID are required." });

      var (gOk, gBody, _) = await _api.GetAsync($"/api/Student/{studentId}");
      if (!gOk) return NotFound(new { message = "Student not found." });

      using var stDoc = JsonDocument.Parse(UnwrapObject(gBody));
      var st = stDoc.RootElement;
      if (st.TryGetProperty("data", out var data) || st.TryGetProperty("Data", out data)) st = data;

      var payload = JsonSerializer.Serialize(new
      {
        user_ID = AmsDataHelper.GetInt(st, "user_ID", "User_ID"),
        program_ID = AmsDataHelper.GetInt(st, "program_ID", "Program_ID"),
        department_ID = AmsDataHelper.GetInt(st, "department_ID", "Department_ID"),
        sectionID = sectionId,
        year_Level = AmsDataHelper.GetInt(st, "year_Level", "Year_Level", "year_Level")
      });

      var (ok, body, status) = await _api.PutAsync($"/api/Student/{studentId}", payload);
      return ok ? Ok(new { message = "Student assigned to section.", section_ID = sectionId }) : StatusCode(status, body);
    }
  }
}
