using Backend.Backend;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Backend.Repository
{
    public class ScheduleRepository(DatabaseLibrary db) : IScheduleRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _db.Schedules.Include(s => s.Course)
            .Include(s => s.Section).ToListAsync();
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _db.Schedules.Include(s => s.Course)
            .Include(s => s.Section).FirstOrDefaultAsync(s => s.Schedule_Id == id);
        }

        public async Task AddAsync(Schedule schedule)
        {
            _db.Schedules.Add(schedule);
            await _db.SaveChangesAsync();
        }

        public async Task<Schedule?> GetScheduleIfExist(string id, DayOfWeek dayOfWeek, TimeOnly now)
        {
            return await _db.Schedules
                .Include(s => s.Course)
                .Include(s => s.Section)
                .Where(s => s.Teacher_ID == id
                    && s.DayOfWeek == dayOfWeek
                    && s.StartTime.AddMinutes(-30) <= now
                    && s.EndTime >= now)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Schedule>> GetTeacherSchedulesForDayAsync(string teacherId, DayOfWeek dayOfWeek)
        {
            return await _db.Schedules
                .Include(s => s.Course)
                .Include(s => s.Section)
                .Where(s => s.Teacher_ID == teacherId && s.DayOfWeek == dayOfWeek)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Gets schedules of a student for a specific day.
        /// </summary>
        /// <param name="studentId">Student ULID/UUID.</param>
        /// <param name="dayOfWeek">Day of week.</param>
        /// <returns>List of schedules.</returns>
        public async Task<IEnumerable<GetStudentSchedule>> GetStudentSchedulesAsync(string studentId, DayOfWeek dayOfWeek)
        {
            var query = from s in _db.Schedules
                        join sec in _db.Sections
                            on s.Section_ID equals sec.Section_Id
                        join c in _db.Courses
                            on s.Course_ID equals c.Course_ID
                        join st in _db.Students
                            on sec.Section_Id equals st.SectionID
                        where st.Student_ID == studentId
                            && s.DayOfWeek == dayOfWeek
                        orderby s.StartTime
                        select new GetStudentSchedule
                        {
                            Title = c.Title,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime
                        };
            return await query.ToListAsync();
        }

        public async Task<bool> HasConflictingScheduleAsync(int courseId,string academicYear,TimeOnly startTime,TimeOnly endTime,int sectionId)
        {
            return await _db.Schedules.AnyAsync(s =>
                s.Course_ID == courseId
                && s.AcademicYear == academicYear
                && s.StartTime == startTime
                && s.EndTime == endTime
                && s.Section_ID != sectionId);
        }

        public async Task UpdateAsync(Schedule schedule)
        {
            _db.Entry(schedule).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Schedule schedule)
        {
            _db.Schedules.Remove(schedule);
            await _db.SaveChangesAsync();
        }
    }
}