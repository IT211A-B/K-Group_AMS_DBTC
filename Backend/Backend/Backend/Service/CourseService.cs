using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Helper;

namespace Backend.Backend.Service
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ISectionRepository _sectionRepository;

        public CourseService(ICourseRepository courseRepository, ISectionRepository sectionRepository)
        {
            _courseRepository = courseRepository;
            _sectionRepository = sectionRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetCourseDTO>>> GetAllAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            if (courses is null || !courses.Any())
                return new ResponseDTO<IEnumerable<GetCourseDTO>> { Status_code = 404, Data = null };

            var data = courses.Select(c => new GetCourseDTO
            {
                Course_ID = c.Course_ID,
                Title = c.Title,
                Code = c.Code,
                Description = c.Description,
                Section_ID = c.Sections.FirstOrDefault()?.Section_Id,
                Section_Code = c.Sections.FirstOrDefault()?.Section_Code
            });

            return new ResponseDTO<IEnumerable<GetCourseDTO>> { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetCourseDTO>> GetByIdAsync(int id)
        {
            var c = await _courseRepository.GetByIdAsync(id);
            if (c == null)
                return new ResponseDTO<GetCourseDTO> { Status_code = 404, Data = null };

            var data = new GetCourseDTO
            {
                Course_ID = c.Course_ID,
                Title = c.Title,
                Code = c.Code,
                Description = c.Description,
                Section_ID = c.Sections.FirstOrDefault()?.Section_Id,
                Section_Code = c.Sections.FirstOrDefault()?.Section_Code
            };

            return new ResponseDTO<GetCourseDTO> { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetCourseDTO>> AddAsync(AddCourseDTO dto)
        {
            var section = await _sectionRepository.GetByIdAsync(dto.Section_ID);
            if (section == null)
                throw new Exception($"Section Id {dto.Section_ID} does not exist");

            var course = new Course
            {
                Title = dto.Title,
                Code = dto.Code,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            await _courseRepository.AddAsync(course);

            section.Course_ID = course.Course_ID;
            await _sectionRepository.UpdateAsync(section);

            var data = new GetCourseDTO
            {
                Course_ID = course.Course_ID,
                Title = course.Title,
                Code = course.Code,
                Description = course.Description,
                Section_ID = section.Section_Id,
                Section_Code = section.Section_Code
            };

            return new ResponseDTO<GetCourseDTO> { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetCourseDTO>> UpdateAsync(int id, AddCourseDTO dto)
        {
            var existing = await _courseRepository.GetByIdAsync(id);
            if (existing == null) return new ResponseDTO<GetCourseDTO> { Status_code = 404, Data = null };

            existing.Title = dto.Title;
            existing.Code = dto.Code;
            existing.Description = dto.Description;
            existing.LastUpdatedAt = TimeHelper.Now();

            await _courseRepository.UpdateAsync(existing);

            var section = await _sectionRepository.GetByIdAsync(dto.Section_ID);
            if (section != null)
            {
                section.Course_ID = existing.Course_ID;
                await _sectionRepository.UpdateAsync(section);
            }

            var data = new GetCourseDTO
            {
                Course_ID = existing.Course_ID,
                Title = existing.Title,
                Code = existing.Code,
                Description = existing.Description,
                Section_ID = section?.Section_Id,
                Section_Code = section?.Section_Code
            };

            return new ResponseDTO<GetCourseDTO> { Status_code = 200, Data = data };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _courseRepository.GetByIdAsync(id);
            if (existing == null) return false;
            await _courseRepository.DeleteAsync(existing);
            return true;
        }
    }
}
