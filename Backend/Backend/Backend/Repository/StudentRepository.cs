using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Model;
using Backend.Backend.Repository;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Backend.Backend.Repository
{
    public class StudentRepository(DatabaseLibrary db) : IStudentRepository
    {
        private readonly DatabaseLibrary _db = db;

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _db.Students.Include(s => s.User).ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int ID)
        {
            return await _db.Students
                .FromSqlRaw(@"SELECT * FROM ""Students"" 
                  WHERE CAST(SPLIT_PART(""DocumentSeries"", '-', 3) AS INT) = {0}", ID).Include(s => s.User)
                .FirstOrDefaultAsync();
        }
        public async Task<Student?> GetByUUIDAsync(string id)
        {
            return await _db.Students
                .Include(s => s.User)
                .FirstOrDefaultAsync(fid => fid.Student_ID == id);
        }

        public async Task<Student?> GetByUserUUIDAsync(string id)
        {
            return await _db.Students
                .Include(s => s.User)
                .Where(s => s.User_ID  == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Student?> GetByQrToken(string qrToken)
        {
            return await _db.Students
                .Include(s => s.User)
                .Where(s => s.QrToken == qrToken)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Student student)
        {
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _db.Entry(student).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Student student)
        {
            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
        }

        public async Task<Program_?> GetProgramByIdAsync(int id)
        {
            return await _db.Programs.FindAsync(id);
        }

        /// <summary>
        /// Gets attendance records of a student.
        /// </summary>
        /// <param name="studentId">Student ULID/UUID.</param>
        /// <returns>List of attendance details.</returns>
        public async Task<List<GetRecordAttendanceOfCertainStudent>> GetStudentAttendanceAsync(string studentId)
        {
            return await _db.AttendanceStudents
                .Join(
                    _db.Students,
                    sa => sa.Student_Id,         
                    st => st.Student_ID,         
                    (sa, st) => new { sa, st }   
                )
                .Join(
                    _db.Attendances,
                    x => x.sa.Attendance_Id,
                    a => a.Attendance_ID,
                    (x, a) => new { x.sa, x.st, a }
                )
                .Join(
                    _db.Schedules,
                    x => x.a.Schedule_ID,
                    s => s.Schedule_Id,
                    (x, s) => new { x.sa, x.st, x.a, s }
                )
                .Join(
                    _db.Courses,
                    x => x.s.Course_ID,
                    c => c.Course_ID,
                    (x, c) => new { x.sa,x.st,x.a,x.s,c }
                )
                .Where(x => x.st.Student_ID == studentId)
                .OrderByDescending(x => x.a.Attendance_ID)
                .Select(x => new GetRecordAttendanceOfCertainStudent
                {
                    Attendance_ID = x.a.Attendance_ID,
                    Course_Title = x.c.Title,
                    Course_Code = x.c.Code,
                    Date = x.a.Date,
                    AttendanceStatus = x.sa.StudentAttendance,
                    DayOfWeek = x.s.DayOfWeek
                })
                .ToListAsync<GetRecordAttendanceOfCertainStudent>();
        }

        public async Task<long> GetNextStudentNumber()
        {
            return await _db.Database
                .SqlQuery<long>($"SELECT nextval('StudentSeq') AS \"Value\"")
                .SingleAsync();
        }

        public async Task<bool> CheckUserIfTaken(string uId)
        {
            return await _db.Students.AnyAsync(s => s.User_ID == uId);
        }

        public async Task<IEnumerable<Student>> GetBySectionIdAsync(int sectionId)
        {
            return await _db.Students
                .Include(s => s.User)
                .Where(s => s.SectionID == sectionId)
                .ToListAsync();
        }
    }
}