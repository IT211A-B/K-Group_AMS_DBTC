using Attendance_Management_System.AttendanceManagementSystem.Model;
using Attendance_Management_System.AttendanceManagementSystem.DTOs;
using Attendance_Management_System.AttendanceManagementSystem.Interface.ServiceInterface;
using Attendance_Management_System.AttendanceManagementSystem.Interface.RepositoryInterface;

namespace Attendance_Management_System.AttendanceManagementSystem.Service
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<GetEnrollmentDTO>> GetAllAsync()
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();
            return enrollments.Select(e => new GetEnrollmentDTO
            {
                Enrollment_ID = e.Enrollment_ID,
                Student_ID = e.Student_ID,
                Schedule_ID = e.Schedule_ID
            });
        }

        public async Task<GetEnrollmentDTO?> GetByIdAsync(int id)
        {
            var e = await _enrollmentRepository.GetByIdAsync(id);
            if (e == null) return null;

            return new GetEnrollmentDTO
            {
                Enrollment_ID = e.Enrollment_ID,
                Student_ID = e.Student_ID,
                Schedule_ID = e.Schedule_ID
            };
        }

        public async Task<GetEnrollmentDTO> AddAsync(AddEnrollmentDTO dto)
        {
            var enrollment = new Enrollment
            {
                Student_ID = dto.Student_ID,
                Schedule_ID = dto.Schedule_ID
            };

            await _enrollmentRepository.AddAsync(enrollment);

            return new GetEnrollmentDTO
            {
                Enrollment_ID = enrollment.Enrollment_ID,
                Student_ID = enrollment.Student_ID,
                Schedule_ID = enrollment.Schedule_ID
            };
        }

        public async Task<GetEnrollmentDTO?> UpdateAsync(int id, AddEnrollmentDTO dto)
        {
            var existing = await _enrollmentRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Student_ID = dto.Student_ID;
            existing.Schedule_ID = dto.Schedule_ID;

            await _enrollmentRepository.UpdateAsync(existing);

            return new GetEnrollmentDTO
            {
                Enrollment_ID = existing.Enrollment_ID,
                Student_ID = existing.Student_ID,
                Schedule_ID = existing.Schedule_ID
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _enrollmentRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _enrollmentRepository.DeleteAsync(existing);
            return true;
        }
    }
}