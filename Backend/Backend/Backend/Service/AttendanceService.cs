using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;
using attStat = Backend.Backend.Helper.Enum.AttendanceEnum.AttStatus;

namespace Backend.Backend.Service
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IScheduleRepository _scheduleRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository, IUserRepository userRepository, IScheduleRepository scheduleRepository)
        {
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
            _scheduleRepository = scheduleRepository;
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

        public async Task<ResponseDTO<GetAttendanceDTO>> AddAsync(AddAttendanceDTO dto, string currentUserId)
        {
            var getOperator = await _userRepository.GetByUUIDAsync(currentUserId);

            // Get schedule
            var getSchedule = await _scheduleRepository.GetByIdAsync(dto.Schedule_ID);
            if (getSchedule == null)
                throw new Exception($"Schedule Id {dto.Schedule_ID} Does not Exist");

            // get current time for validations
            TimeOnly validationTimeAttendanceStatus = TimeOnly.FromDateTime(DateTime.UtcNow);

            // Get time started
            TimeOnly started = getSchedule.StartTime;

            // set late limitation
            TimeOnly lateChecker = validationTimeAttendanceStatus.AddMinutes(15);

            // set attendance status, initialize to absent
            attStat stat = attStat.Absent;

            // get day of the week
            DateTime thisday = DateTime.UtcNow;
            DayOfWeek dayOfThisWeek = thisday.DayOfWeek;

            // Validation and Status Assignment
            // Check if todays is the recorded day for schedule
            if (getSchedule.DayOfWeek != dayOfThisWeek)
                throw new Exception($"Course is Only available at {getSchedule.DayOfWeek} Not {dayOfThisWeek}");

            // Present
            if (validationTimeAttendanceStatus < started)
                stat = attStat.Present;

            if (validationTimeAttendanceStatus <=  lateChecker && validationTimeAttendanceStatus > started)
                stat = attStat.Late;

            if (validationTimeAttendanceStatus > lateChecker)
                stat = attStat.Absent;

            var attendance = new Attendance
            {
                Schedule_ID = dto.Schedule_ID,
                TeacherStatus = stat,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
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

        public async Task<ResponseDTO<GetAttendanceDTO>> UpdateAsync(int id, AddAttendanceDTO dto)
        {
            var existing = await _attendanceRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetAttendanceDTO>()
                {
                    Status_code=404,
                    Data = null
                };

            existing.Schedule_ID = dto.Schedule_ID;

            await _attendanceRepository.UpdateAsync(existing);

            var data = new GetAttendanceDTO
            {
                Attendance_ID = existing.Attendance_ID,
                Schedule_ID = existing.Schedule_ID,
                TeacherStatus = existing.TeacherStatus,
                Date = existing.Date,
                CreatedAt = existing.CreatedAt,
                CreatedBy = existing.CreatedBy,
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