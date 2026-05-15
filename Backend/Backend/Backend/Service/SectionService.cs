using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _sectionRepository;

        public SectionService(ISectionRepository sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetSectionDTO>>> GetAllAsync()
        {
            var sections = await _sectionRepository.GetAllAsync();
            if (sections is null || !sections.Any())
                return new ResponseDTO<IEnumerable<GetSectionDTO>>
                {
                    Status_code= 404,
                    Data = null
                };
            var data = sections.Select(e => new GetSectionDTO
            {
                Section_Id = e.Section_Id,
                Section_Code = e.Section_Code
            });

            return new ResponseDTO<IEnumerable<GetSectionDTO>>
            { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetSectionDTO>> GetByIdAsync(int id)
        {
            var e = await _sectionRepository.GetByIdAsync(id);
            if (e == null)
                return new ResponseDTO<GetSectionDTO>()
                {
                    Status_code = 404,
                    Data = null
                };

            var data = new GetSectionDTO
            {
                Section_Id = e.Section_Id,
                Section_Code = e.Section_Code
            };

            return new ResponseDTO<GetSectionDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetSectionDTO>> AddAsync(AddSectionDTO dto)
        {
            var section = new Section
            {
                Section_Code= dto.Section_Code,
            };

            await _sectionRepository.AddAsync(section);

            var data = new GetSectionDTO
            {
                Section_Code = section.Section_Code,
            };

            return new ResponseDTO<GetSectionDTO>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetSectionDTO>> UpdateAsync(int id, AddSectionDTO dto)
        {
            var existing = await _sectionRepository.GetByIdAsync(id);
            if (existing == null) return new ResponseDTO<GetSectionDTO>() { Status_code = 404, Data = null };

            existing.Section_Code = dto.Section_Code;

            await _sectionRepository.UpdateAsync(existing);

            var data = new GetSectionDTO
            {
                Section_Code = dto.Section_Code,
            };

            return new ResponseDTO<GetSectionDTO>
            {
                Status_code = 200,
                Data = data
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