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

        public AttendanceService(IAttendanceRepository attendanceRepository, IUserRepository userRepository, IScheduleRepository scheduleRepository, ITeacherRepository teacherRepository)
        {
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _scheduleRepository = scheduleRepository;
            _teacherRepository = teacherRepository;
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
    }
}