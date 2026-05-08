using Backend.Backend.DTOs;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Model;

namespace Backend.Backend.Service
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IUserRepository _userRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository, IUserRepository userRepository)
        {
            _attendanceRepository = attendanceRepository;
            _userRepository = userRepository;
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
            var attendance = new Attendance
            {
                Schedule_ID = dto.Schedule_ID,
                TeacherStatus = dto.TeacherStatus,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = getOperator?.Full_Name ?? "Admin"
            };

            await _attendanceRepository.AddAsync(attendance);

            var data = new GetAttendanceDTO
            {
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
            existing.TeacherStatus = dto.TeacherStatus;

            await _attendanceRepository.UpdateAsync(existing);

            var data = new GetAttendanceDTO
            {
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