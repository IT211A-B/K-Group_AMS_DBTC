using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
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
                DocumentSeries = t.DocumentSeries,
                DepartmentId = t.DepartmentId,
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
                DocumentSeries = t.DocumentSeries,
                DepartmentId = t.DepartmentId,
                CreatedAt = t.CreatedAt,
                LastUpdatedAt = t.LastUpdatedAt,
                CreatedBy = t.CreatedBy,
                LastUpdatedBy = t.LastUpdatedBy
            };
        }

        public async Task<GetTeacherDTO> AddAsync(AddTeacherDTO dto)
        {
            // Get Department
            var getDepartment = await _teacherRepository.GetDepartmentById(dto.DepartmentId);
            // Get Acronym
            string getAcronym = Helper.ThreeFirstLetterCapital.CapitalTheFirstThreeLetterInWord(getDepartment!.Name);
            // Get Year
            int Year = DateTime.Now.Year;
            // Get Id
            long getId = await _teacherRepository.GetNextTeacherNumberAsync();

            // Generate Document Series
            string docSer = $"{getAcronym}-{Year}-{getId}";

            var teacher = new Teacher
            {
                DocumentSeries = docSer,
                DepartmentId = dto.DepartmentId,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = dto.LastUpdatedBy
            };

            await _teacherRepository.AddAsync(teacher);

            return new GetTeacherDTO
            {
                DocumentSeries = teacher.DocumentSeries,
                DepartmentId = teacher.DepartmentId,
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

            existing.DepartmentId = dto.DepartmentId;
            existing.LastUpdatedAt = DateTime.UtcNow;
            existing.LastUpdatedBy = dto.LastUpdatedBy;

            await _teacherRepository.UpdateAsync(existing);

            return new GetTeacherDTO
            {
                DepartmentId = existing.DepartmentId,
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