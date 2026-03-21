using Attendance_Management_System.AttendanceManagementSystem.Model;
using Attendance_Management_System.AttendanceManagementSystem.DTOs;
using Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface;
using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Service
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;

        public TeacherService(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<IEnumerable<GetTeacherDTO>> GetAllAsync()
        {
            var teachers = await _teacherRepository.GetAllAsync();
            return teachers.Select(t => new GetTeacherDTO
            {
                Teacher_ID = t.Teacher_ID,
                Department = t.Department,
                CreatedAt = t.CreatedAt,
                LastUpdatedAt = t.LastUpdatedAt,
                CreatedBy = t.CreatedBy,
                LastUpdatedBy = t.LastUpdatedBy
            });
        }

        public async Task<GetTeacherDTO?> GetByIdAsync(int id)
        {
            var t = await _teacherRepository.GetByIdAsync(id);
            if (t == null) return null;

            return new GetTeacherDTO
            {
                Teacher_ID = t.Teacher_ID,
                Department = t.Department,
                CreatedAt = t.CreatedAt,
                LastUpdatedAt = t.LastUpdatedAt,
                CreatedBy = t.CreatedBy,
                LastUpdatedBy = t.LastUpdatedBy
            };
        }

        public async Task<GetTeacherDTO> AddAsync(AddTeacherDTO dto)
        {
            var teacher = new Teacher
            {
                Department = dto.Department,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = dto.LastUpdatedBy
            };

            await _teacherRepository.AddAsync(teacher);

            return new GetTeacherDTO
            {
                Teacher_ID = teacher.Teacher_ID,
                Department = teacher.Department,
                CreatedAt = teacher.CreatedAt,
                LastUpdatedAt = teacher.LastUpdatedAt,
                CreatedBy = teacher.CreatedBy,
                LastUpdatedBy = teacher.LastUpdatedBy
            };
        }

        public async Task<GetTeacherDTO?> UpdateAsync(int id, AddTeacherDTO dto)
        {
            var existing = await _teacherRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Department = dto.Department;
            existing.LastUpdatedAt = DateTime.UtcNow;
            existing.LastUpdatedBy = dto.LastUpdatedBy;

            await _teacherRepository.UpdateAsync(existing);

            return new GetTeacherDTO
            {
                Teacher_ID = existing.Teacher_ID,
                Department = existing.Department,
                CreatedAt = existing.CreatedAt,
                LastUpdatedAt = existing.LastUpdatedAt,
                CreatedBy = existing.CreatedBy,
                LastUpdatedBy = existing.LastUpdatedBy
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _teacherRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _teacherRepository.DeleteAsync(existing);
            return true;
        }
    }
}