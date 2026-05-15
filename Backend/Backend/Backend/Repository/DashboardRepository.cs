using Backend.Backend;
using Backend.Backend.DTOs;
using Backend.Backend.Helper;
using Backend.Backend.Interface.RepositoryInterface;
using Microsoft.EntityFrameworkCore;
using attStat = Backend.Backend.Helper.Enum.AttendanceEnum.AttStatus;

namespace Backend.Backend.Repository
{
    public class DashboardRepository(DatabaseLibrary db) : IDashboardRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<DashboardStatsDTO> GetStatsAsync()
        {
            var today = DateOnly.FromDateTime(TimeHelper.Now());
            var studentAtt = await _db.AttendanceStudents
                .Include(a => a.Attendance)
                .Where(a => a.Attendance.Date == today)
                .ToListAsync();

            return new DashboardStatsDTO
            {
                TotalStudents = await _db.Students.CountAsync(),
                TotalTeachers = await _db.Teachers.CountAsync(),
                TotalCourses = await _db.Courses.CountAsync(),
                AbsencesToday = studentAtt.Count(a => a.StudentAttendance == attStat.Absent),
                PresentToday = studentAtt.Count(a => a.StudentAttendance == attStat.Present),
                LateToday = studentAtt.Count(a => a.StudentAttendance == attStat.Late),
            };
        }
    }
}
