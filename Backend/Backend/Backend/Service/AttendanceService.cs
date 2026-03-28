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

        public async Task<IEnumerable<GetAttendanceDTO>> GetAllAsync()
        {
            var attendances = await _attendanceRepository.GetAllAsync();
            return attendances.Select(a => new GetAttendanceDTO
            {
                Attendance_ID = a.Attendance_ID,
                Enrollment_ID = a.Enrollment_ID,
                Date = a.Date,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                LastUpdatedAt = a.LastUpdatedAt,
                CreatedBy = a.CreatedBy,
                LastUpdatedBy = a.LastUpdatedBy
            });
        }

        public async Task<GetAttendanceDTO?> GetByIdAsync(int id)
        {
            var a = await _attendanceRepository.GetByIdAsync(id);
            if (a == null) return null;

            return new GetAttendanceDTO
            {
                Attendance_ID = a.Attendance_ID,
                Enrollment_ID = a.Enrollment_ID,
                Date = a.Date,
                Status = a.Status,
                CreatedAt = a.CreatedAt,
                LastUpdatedAt = a.LastUpdatedAt,
                CreatedBy = a.CreatedBy,
                LastUpdatedBy = a.LastUpdatedBy
            };
        }

        public async Task<GetAttendanceDTO> AddAsync(AddAttendanceDTO dto)
        {
            var attendance = new Attendance
            {
                Enrollment_ID = dto.Enrollment_ID,
                Date = dto.Date,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = dto.LastUpdatedBy
            };

            await _attendanceRepository.AddAsync(attendance);

            return new GetAttendanceDTO
            {
                Attendance_ID = attendance.Attendance_ID,
                Enrollment_ID = attendance.Enrollment_ID,
                Date = attendance.Date,
                Status = attendance.Status,
                CreatedAt = attendance.CreatedAt,
                LastUpdatedAt = attendance.LastUpdatedAt,
                CreatedBy = attendance.CreatedBy,
                LastUpdatedBy = attendance.LastUpdatedBy
            };
        }

        public async Task<GetAttendanceDTO?> UpdateAsync(int id, AddAttendanceDTO dto)
        {
            var existing = await _attendanceRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Enrollment_ID = dto.Enrollment_ID;
            existing.Date = dto.Date;
            existing.Status = dto.Status;
            existing.LastUpdatedAt = DateTime.UtcNow;
            existing.LastUpdatedBy = dto.LastUpdatedBy;

            await _attendanceRepository.UpdateAsync(existing);

            return new GetAttendanceDTO
            {
                Attendance_ID = existing.Attendance_ID,
                Enrollment_ID = existing.Enrollment_ID,
                Date = existing.Date,
                Status = existing.Status,
                CreatedAt = existing.CreatedAt,
                LastUpdatedAt = existing.LastUpdatedAt,
                CreatedBy = existing.CreatedBy,
                LastUpdatedBy = existing.LastUpdatedBy
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