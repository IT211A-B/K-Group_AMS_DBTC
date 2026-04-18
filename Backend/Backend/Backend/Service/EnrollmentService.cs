using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetEnrollmentDTO>>> GetAllAsync()
        {
            var enrollments = await _enrollmentRepository.GetAllAsync();
            if (enrollments is null || !enrollments.Any())
                return new ResponseDTO<IEnumerable<GetEnrollmentDTO>>
                {
                    Status_code= 404,
                    Data = null
                };
            var data = enrollments.Select(e => new GetEnrollmentDTO
            {
                Enrollment_ID = e.Enrollment_ID,
                Student_ID = e.Student_ID,
                Schedule_ID = e.Schedule_ID
            });

            return new ResponseDTO<IEnumerable<GetEnrollmentDTO>>
            { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetEnrollmentDTO>> GetByIdAsync(int id)
        {
            var e = await _enrollmentRepository.GetByIdAsync(id);
            if (e == null)
                return new ResponseDTO<GetEnrollmentDTO>()
                {
                    Status_code = 404,
                    Data = null
                };

            var data = new GetEnrollmentDTO
            {
                Enrollment_ID = e.Enrollment_ID,
                Student_ID = e.Student_ID,
                Schedule_ID = e.Schedule_ID
            };

            return new ResponseDTO<GetEnrollmentDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetEnrollmentDTO>> AddAsync(AddEnrollmentDTO dto)
        {
            var enrollment = new Enrollment
            {
                Student_ID = dto.Student_ID,
                Schedule_ID = dto.Schedule_ID
            };

            await _enrollmentRepository.AddAsync(enrollment);

            var data = new GetEnrollmentDTO
            {
                Enrollment_ID = enrollment.Enrollment_ID,
                Student_ID = enrollment.Student_ID,
                Schedule_ID = enrollment.Schedule_ID
            };

            return new ResponseDTO<GetEnrollmentDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetEnrollmentDTO>> UpdateAsync(int id, AddEnrollmentDTO dto)
        {
            var existing = await _enrollmentRepository.GetByIdAsync(id);
            if (existing == null) return new ResponseDTO<GetEnrollmentDTO>() { Status_code = 404, Data = null };

            existing.Student_ID = dto.Student_ID;
            existing.Schedule_ID = dto.Schedule_ID;

            await _enrollmentRepository.UpdateAsync(existing);

            var data = new GetEnrollmentDTO
            {
                Enrollment_ID = existing.Enrollment_ID,
                Student_ID = existing.Student_ID,
                Schedule_ID = existing.Schedule_ID
            };

            return new ResponseDTO<GetEnrollmentDTO>
            {
                Status_code = 200,
                Data = data
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