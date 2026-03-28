using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<GetCourseDTO>> GetAllAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses.Select(c => new GetCourseDTO
            {
                Course_ID = c.Course_ID,
                Title = c.Title,
                Code = c.Code,
                Description = c.Description,
                Teacher_ID = c.Teacher_ID,
                CreatedAt = c.CreatedAt,
                LastUpdatedAt = c.LastUpdatedAt,
                CreatedBy = c.CreatedBy,
                LastUpdatedBy = c.LastUpdatedBy
            });
        }

        public async Task<GetCourseDTO?> GetByIdAsync(int id)
        {
            var c = await _courseRepository.GetByIdAsync(id);
            if (c == null) return null;

            return new GetCourseDTO
            {
                Course_ID = c.Course_ID,
                Title = c.Title,
                Code = c.Code,
                Description = c.Description,
                Teacher_ID = c.Teacher_ID,
                CreatedAt = c.CreatedAt,
                LastUpdatedAt = c.LastUpdatedAt,
                CreatedBy = c.CreatedBy,
                LastUpdatedBy = c.LastUpdatedBy
            };
        }

        public async Task<GetCourseDTO> AddAsync(AddCourseDTO dto)
        {
            var course = new Course
            {
                Title = dto.Title,
                Code = dto.Code,
                Description = dto.Description,
                Teacher_ID = dto.Teacher_ID,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                LastUpdatedBy = dto.LastUpdatedBy
            };

            await _courseRepository.AddAsync(course);

            return new GetCourseDTO
            {
                Course_ID = course.Course_ID,
                Title = course.Title,
                Code = course.Code,
                Description = course.Description,
                Teacher_ID = course.Teacher_ID,
                CreatedAt = course.CreatedAt,
                LastUpdatedAt = course.LastUpdatedAt,
                CreatedBy = course.CreatedBy,
                LastUpdatedBy = course.LastUpdatedBy
            };
        }

        public async Task<GetCourseDTO?> UpdateAsync(int id, AddCourseDTO dto)
        {
            var existing = await _courseRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Title = dto.Title;
            existing.Code = dto.Code;
            existing.Description = dto.Description;
            existing.Teacher_ID = dto.Teacher_ID;
            existing.LastUpdatedAt = DateTime.UtcNow;
            existing.LastUpdatedBy = dto.LastUpdatedBy;

            await _courseRepository.UpdateAsync(existing);

            return new GetCourseDTO
            {
                Course_ID = existing.Course_ID,
                Title = existing.Title,
                Code = existing.Code,
                Description = existing.Description,
                Teacher_ID = existing.Teacher_ID,
                CreatedAt = existing.CreatedAt,
                LastUpdatedAt = existing.LastUpdatedAt,
                CreatedBy = existing.CreatedBy,
                LastUpdatedBy = existing.LastUpdatedBy
            };
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