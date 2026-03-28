using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<GetStudentDTO>> GetAllAsync()
        {
            var students = await _studentRepository.GetAllAsync();
            return students.Select(s => new GetStudentDTO
            {
                Student_ID = s.Student_ID,
                Program_ID = s.Program_ID,
                Department_ID = s.Department_ID,
                Year_Level = s.Year_Level,
                CreatedAt = s.CreatedAt,
                LastUpdatedAt = s.LastUpdatedAt,
                CreatedBy = s.CreatedBy,
                LastUpdatedBy = s.LastUpdatedBy
            });
        }

        public async Task<GetStudentDTO?> GetByIdAsync(int id)
        {
            var s = await _studentRepository.GetByIdAsync(id);
            if (s == null) return null;

            return new GetStudentDTO
            {
                Student_ID = s.Student_ID,
                Program_ID = s.Program_ID,
                Department_ID = s.Department_ID,
                Year_Level = s.Year_Level,
                CreatedAt = s.CreatedAt,
                LastUpdatedAt = s.LastUpdatedAt,
                CreatedBy = s.CreatedBy,
                LastUpdatedBy = s.LastUpdatedBy
            };
        }

        public async Task<GetStudentDTO> AddAsync(AddStudentDTO dto)
        {
            var student = new Student
            {
                Program_ID = dto.Program_ID,
                Department_ID = dto.Department_ID,
                Year_Level = dto.Year_Level,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = dto.LastUpdatedBy
            };

            await _studentRepository.AddAsync(student);

            return new GetStudentDTO
            {
                Student_ID = student.Student_ID,
                Program_ID = student.Program_ID,
                Department_ID = student.Department_ID,
                Year_Level = student.Year_Level,
                CreatedAt = student.CreatedAt,
                LastUpdatedAt = student.LastUpdatedAt,
                CreatedBy = student.CreatedBy,
                LastUpdatedBy = student.LastUpdatedBy
            };
        }

        public async Task<GetStudentDTO?> UpdateAsync(int id, AddStudentDTO dto)
        {
            var existing = await _studentRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Program_ID = dto.Program_ID;
            existing.Department_ID = dto.Department_ID;
            existing.Year_Level = dto.Year_Level;
            existing.LastUpdatedAt = DateTime.UtcNow;
            existing.LastUpdatedBy = dto.LastUpdatedBy;

            await _studentRepository.UpdateAsync(existing);

            return new GetStudentDTO
            {
                Student_ID = existing.Student_ID,
                Program_ID = existing.Program_ID,
                Department_ID = existing.Department_ID,
                Year_Level = existing.Year_Level,
                CreatedAt = existing.CreatedAt,
                LastUpdatedAt = existing.LastUpdatedAt,
                CreatedBy = existing.CreatedBy,
                LastUpdatedBy = existing.LastUpdatedBy
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _studentRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _studentRepository.DeleteAsync(existing);
            return true;
        }
    }
}