using Backend.Backend.DTOs;
using Backend.Backend.Helper;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using attStat = Backend.Backend.Helper.Enum.AttendanceEnum.AttStatus;

namespace Backend.Backend.Service
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentRepository _studentRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository, IUserRepository userRepository, IScheduleRepository scheduleRepository, ITeacherRepository teacherRepository, IStudentRepository studentRepository)
        {
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _scheduleRepository = scheduleRepository;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetAttendanceDTO>>> GetAllAsync()
        {
            var attendances = await _attendanceRepository.GetAllAsync();
            if (attendances is null || !attendances.Any())
                return new ResponseDTO<IEnumerable<GetAttendanceDTO>>
                {
                    Status_code= 404,
                    Data = null
                };
            var data = attendances.Select(a => new GetAttendanceDTO
            {
                Attendance_ID = a.Attendance_ID,
                Schedule_ID = a.Schedule_ID,
                TeacherStatus = a.TeacherStatus,
                Date = a.Date,
                CreatedAt = a.CreatedAt,
                CreatedBy = a.CreatedBy,
            });

            return new ResponseDTO<IEnumerable<GetAttendanceDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetAttendanceDTO>> GetByIdAsync(int id)
        {
            var a = await _attendanceRepository.GetByIdAsync(id);
            if (a == null)
                return new ResponseDTO<GetAttendanceDTO>()
                {
                    Status_code =404,
                    Data = null
                };

            var data = new GetAttendanceDTO
            {
                Attendance_ID= a.Attendance_ID,
                Schedule_ID = a.Schedule_ID,
                TeacherStatus = a.TeacherStatus,
                Date = a.Date,
                CreatedAt = a.CreatedAt,
                CreatedBy = a.CreatedBy,
            };

            return new ResponseDTO<GetAttendanceDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetAttendanceDTO>> AddAsync(string currentUserId)
        {
            var getOperator = await _userRepository.GetByUUIDAsync(currentUserId);
            if (getOperator == null)
                throw new Exception("No Operator Has been Found");
            Console.WriteLine(currentUserId);
            var getTeacher = await _teacherRepository.GetTeacherByUserUUIDAsync(currentUserId);
            if (getTeacher == null)
                throw new Exception("No Teacher Has Been Found");

            // get current time for validations
            TimeOnly now = TimeOnly.FromDateTime(TimeHelper.Now());

            // set attendance status, initialize to absent
            attStat stat = attStat.Unassigned;

            // get day of the week
            DateOnly thisday = DateOnly.FromDateTime(TimeHelper.Now());
            DayOfWeek dayOfThisWeek = thisday.DayOfWeek;

            var getSchedule = await _scheduleRepository.GetScheduleIfExist(getTeacher.Teacher_ID, dayOfThisWeek, now);
            if (getSchedule == null)
                throw new Exception($"You do not have any classes in this hour");

            // Limit Time that can be tracked
            TimeOnly attendanceStarted = getSchedule.StartTime.AddMinutes(-30);

            // Get time started
            TimeOnly started = getSchedule.StartTime;

            // set late limitation
            TimeOnly lateChecker = started.AddMinutes(15);

            // Validation and Status Assignment
            // Check if todays is the recorded day for schedule
            if (getSchedule.DayOfWeek != dayOfThisWeek)
                throw new Exception($"Course is Only available at {getSchedule.DayOfWeek} Not {dayOfThisWeek}");


            // Validation and Status Assignment
            // Check if todays is the recorded day for schedule
            if (getSchedule.DayOfWeek != dayOfThisWeek)
                throw new Exception($"Course is Only available at {getSchedule.DayOfWeek} Not {dayOfThisWeek}");

            // Status
            if (now <= started && attendanceStarted >= now)
                stat = attStat.Present;
            else if (now <= lateChecker)
                stat = attStat.Late;
            else if (now > lateChecker)
                stat = attStat.Absent;
            else
                stat = attStat.Unassigned;

            var attendance = new Attendance
            {
                Schedule_ID = getSchedule.Schedule_Id,
                TeacherStatus = stat,
                Date = DateOnly.FromDateTime(TimeHelper.Now()),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = getOperator?.Full_Name ?? "Admin"
            };

            await _attendanceRepository.AddAsync(attendance);

            var data = new GetAttendanceDTO
            {
                Attendance_ID= attendance.Attendance_ID,
                Schedule_ID = attendance.Schedule_ID,
                TeacherStatus = attendance.TeacherStatus,
                Date = attendance.Date,
                CreatedAt = attendance.CreatedAt,
                CreatedBy = attendance.CreatedBy,
            };

            return new ResponseDTO<GetAttendanceDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _attendanceRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _attendanceRepository.DeleteAsync(existing);
            return true;
        }

        public async Task<ResponseDTO<IEnumerable<GetTeacherScheduleDTO>>> GetTeacherSchedulesAsync(string currentUserId)
        {
            var teacher = await _teacherRepository.GetTeacherByUserUUIDAsync(currentUserId);
            if (teacher == null)
                throw new Exception("No Teacher Has Been Found");

            var today = DateOnly.FromDateTime(TimeHelper.Now());
            var dayOfWeek = today.DayOfWeek;
            var now = TimeOnly.FromDateTime(TimeHelper.Now());

            var schedules = await _scheduleRepository.GetTeacherSchedulesForDayAsync(teacher.Teacher_ID, dayOfWeek);
            var data = schedules.Select(s => new GetTeacherScheduleDTO
            {
                Schedule_Id = s.Schedule_Id,
                Course_ID = s.Course_ID,
                Course_Code = s.Course.Code,
                Course_Title = s.Course.Title,
                Section_ID = s.Section_ID,
                Section_Code = s.Section.Section_Code,
                DayOfWeek = s.DayOfWeek,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                IsActiveNow = s.StartTime.AddMinutes(-30) <= now && s.EndTime >= now
            });

            return new ResponseDTO<IEnumerable<GetTeacherScheduleDTO>> { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<IEnumerable<GetSessionStudentDTO>>> GetSessionStudentsAsync(string currentUserId)
        {
            var teacher = await _teacherRepository.GetTeacherByUserUUIDAsync(currentUserId);
            if (teacher == null)
                throw new Exception("No Teacher Has Been Found");

            var today = DateOnly.FromDateTime(TimeHelper.Now());
            var now = TimeOnly.FromDateTime(TimeHelper.Now());
            var schedule = await _scheduleRepository.GetScheduleIfExist(teacher.Teacher_ID, today.DayOfWeek, now);
            if (schedule == null)
                return new ResponseDTO<IEnumerable<GetSessionStudentDTO>> { Status_code = 200, Data = [] };

            var attendance = await _attendanceRepository.GetTodayByScheduleAsync(schedule.Schedule_Id, today);
            var students = await _studentRepository.GetBySectionIdAsync(schedule.Section_ID);

            var marked = attendance?.AttendanceStudents?.ToDictionary(a => a.Student_Id, a => a.StudentAttendance.ToString()) ?? new Dictionary<string, string>();

            var data = students.Select(s => new GetSessionStudentDTO
            {
                Student_Id = s.Student_ID,
                Student_Name = s.User.Full_Name,
                DocumentSeries = s.DocumentSeries,
                AttendanceStatus = marked.TryGetValue(s.Student_ID, out var st) ? st : null
            });

            return new ResponseDTO<IEnumerable<GetSessionStudentDTO>> { Status_code = 200, Data = data };
        }
    }
}