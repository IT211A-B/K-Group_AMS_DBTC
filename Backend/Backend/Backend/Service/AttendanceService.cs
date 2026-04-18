using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
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
                Enrollment_ID = a.Enrollment_ID,
                Date = a.Date,
                Status = a.Status,
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
                Attendance_ID = a.Attendance_ID,
                Enrollment_ID = a.Enrollment_ID,
                Date = a.Date,
                Status = a.Status,
            };

            return new ResponseDTO<GetAttendanceDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetAttendanceDTO>> AddAsync(AddAttendanceDTO dto)
        {
            var attendance = new Attendance
            {
                Enrollment_ID = dto.Enrollment_ID,
                Date = dto.Date,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            await _attendanceRepository.AddAsync(attendance);

            var data = new GetAttendanceDTO
            {
                Attendance_ID = attendance.Attendance_ID,
                Enrollment_ID = attendance.Enrollment_ID,
                Date = attendance.Date,
                Status = attendance.Status,
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

            existing.Enrollment_ID = dto.Enrollment_ID;
            existing.Date = dto.Date;
            existing.Status = dto.Status;
            existing.LastUpdatedAt = DateTime.UtcNow;

            await _attendanceRepository.UpdateAsync(existing);

            var data = new GetAttendanceDTO
            {
                Attendance_ID = existing.Attendance_ID,
                Enrollment_ID = existing.Enrollment_ID,
                Date = existing.Date,
                Status = existing.Status,
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