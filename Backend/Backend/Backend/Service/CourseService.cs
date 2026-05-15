using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;
using Microsoft.AspNetCore.Http.HttpResults;
using Backend.Backend.Helper;

namespace Backend.Backend.Service
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ITeacherRepository _teacherRepository;

        public CourseService(ICourseRepository courseRepository, ITeacherRepository teacherRepository)
        {
            _courseRepository = courseRepository;
            _teacherRepository=teacherRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetCourseDTO>>> GetAllAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            if (courses is null || !courses.Any())
                return new ResponseDTO<IEnumerable<GetCourseDTO>>
                {
                    Status_code= 404,
                    Data = null
                };
            var data = courses.Select(c => new GetCourseDTO
            {
                Course_ID = c.Course_ID,
                Title = c.Title,
                Code = c.Code,
                Description = c.Description,
                TeacherDocumentSeries = c.Teacher.DocumentSeries
            });

            return new ResponseDTO<IEnumerable<GetCourseDTO>>()
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetCourseDTO>> GetByIdAsync(int id)
        {
            var c = await _courseRepository.GetByIdAsync(id);
            if (c == null)
                return new ResponseDTO<GetCourseDTO>()
                {
                    Status_code = 200,
                    Data = null
                };

            var data = new GetCourseDTO
            {
                Course_ID = c.Course_ID,
                Title = c.Title,
                Code = c.Code,
                Description = c.Description,
                TeacherDocumentSeries = c.Teacher.DocumentSeries
            };

            return new ResponseDTO<GetCourseDTO>()
                { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetCourseDTO>> AddAsync(AddCourseDTO dto)
        {
            //get Teacher
            var getTeacher = await _teacherRepository.GetByIdAsync(dto.Teacher_ID);
            if (getTeacher == null)
                throw new Exception($"Teacher Id {dto.Teacher_ID} does not Exist");
            var course = new Course
            {
                Title = dto.Title,
                Code = dto.Code,
                Description = dto.Description,
                Teacher_ID = getTeacher.Teacher_ID,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
            };

            await _courseRepository.AddAsync(course);

            var data = new GetCourseDTO
            {
                Course_ID = course.Course_ID,
                Title = course.Title,
                Code = course.Code,
                Description = course.Description,
                TeacherDocumentSeries = course.Teacher.DocumentSeries,
            };

            return new ResponseDTO<GetCourseDTO>()
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetCourseDTO>> UpdateAsync(int id, AddCourseDTO dto)
        {
            var existing = await _courseRepository.GetByIdAsync(id);
            if (existing == null) return new ResponseDTO<GetCourseDTO>() { Status_code = 404, Data = null };

            //get Teacher
            var getTeacher = await _teacherRepository.GetByIdAsync(dto.Teacher_ID);
            if (getTeacher == null)
                throw new Exception($"Teacher Id {dto.Teacher_ID} does not Exist");

            existing.Title = dto.Title;
            existing.Code = dto.Code;
            existing.Description = dto.Description;
            existing.Teacher_ID = getTeacher.Teacher_ID;
            existing.LastUpdatedAt = TimeHelper.Now();

            await _courseRepository.UpdateAsync(existing);

            var data = new GetCourseDTO
            {
                Course_ID = existing.Course_ID,
                Title = existing.Title,
                Code = existing.Code,
                Description = existing.Description,
                TeacherDocumentSeries = existing.Teacher.DocumentSeries,
            };

            return new ResponseDTO<GetCourseDTO>()
                { Status_code = 200,
                Data = data };
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