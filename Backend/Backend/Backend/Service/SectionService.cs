using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _sectionRepository;
        private readonly ICourseRepository _courseRepository;

        public SectionService(ISectionRepository sectionRepository, ICourseRepository courseRepository)
        {
            _sectionRepository = sectionRepository;
            _courseRepository = courseRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetSectionDTO>>> GetAllAsync()
        {
            var sections = await _sectionRepository.GetAllAsync();
            if (sections is null || !sections.Any())
                return new ResponseDTO<IEnumerable<GetSectionDTO>> { Status_code = 404, Data = null };

            var data = sections.Select(e => new GetSectionDTO
            {
                Section_Id = e.Section_Id,
                Section_Code = e.Section_Code,
                Course_ID = e.Course_ID,
                Course_Code = e.Course?.Code,
                Course_Title = e.Course?.Title
            });

            return new ResponseDTO<IEnumerable<GetSectionDTO>> { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetSectionDTO>> GetByIdAsync(int id)
        {
            var e = await _sectionRepository.GetByIdAsync(id);
            if (e == null)
                return new ResponseDTO<GetSectionDTO> { Status_code = 404, Data = null };

            var data = new GetSectionDTO
            {
                Section_Id = e.Section_Id,
                Section_Code = e.Section_Code,
                Course_ID = e.Course_ID,
                Course_Code = e.Course?.Code,
                Course_Title = e.Course?.Title
            };

            return new ResponseDTO<GetSectionDTO> { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetSectionDTO>> AddAsync(AddSectionDTO dto)
        {
            var course = await _courseRepository.GetByIdAsync(dto.Course_ID);
            if (course == null)
                throw new Exception($"Course {dto.Course_ID} does not exist");

            var section = new Section
            {
                Section_Code = dto.Section_Code,
                Course_ID = dto.Course_ID
            };

            await _sectionRepository.AddAsync(section);

            return new ResponseDTO<GetSectionDTO>
            {
                Status_code = 200,
                Data = new GetSectionDTO
                {
                    Section_Id = section.Section_Id,
                    Section_Code = section.Section_Code,
                    Course_ID = section.Course_ID,
                    Course_Code = course.Code,
                    Course_Title = course.Title
                }
            };
        }

        public async Task<ResponseDTO<GetSectionDTO>> UpdateAsync(int id, AddSectionDTO dto)
        {
            var existing = await _sectionRepository.GetByIdAsync(id);
            if (existing == null) return new ResponseDTO<GetSectionDTO> { Status_code = 404, Data = null };

            existing.Section_Code = dto.Section_Code;
            existing.Course_ID = dto.Course_ID;

            await _sectionRepository.UpdateAsync(existing);

            return new ResponseDTO<GetSectionDTO>
            {
                Status_code = 200,
                Data = new GetSectionDTO
                {
                    Section_Id = existing.Section_Id,
                    Section_Code = existing.Section_Code,
                    Course_ID = existing.Course_ID
                }
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _sectionRepository.GetByIdAsync(id);
            if (existing == null) return false;
            await _sectionRepository.DeleteAsync(existing);
            return true;
        }
    }
}
