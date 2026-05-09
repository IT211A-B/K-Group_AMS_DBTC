using Backend.Backend.Model;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Interface.RepositoryInterface;

namespace Backend.Backend.Service
{
    public class ProgramService : IProgramService
    {
        private readonly IProgramRepository _programRepository;

        public ProgramService(IProgramRepository programRepository)
        {
            _programRepository = programRepository;
        }

        public async Task<ResponseDTO<IEnumerable<GetProgramDTO>>> GetAllAsync()
        {
            var programs = await _programRepository.GetAllAsync();
            if (!programs.Any() || programs is null)
                return new ResponseDTO<IEnumerable<GetProgramDTO>>
                {
                    Status_code = 404,
                    Data = null
                };

            var data = programs.Select(p => new GetProgramDTO
            {
                Program_Id = p.Program_Id,
                Name = p.Name,
                Description = p.Description
            });

            return new ResponseDTO<IEnumerable<GetProgramDTO>>
            {
                Status_code = 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetProgramDTO>> GetByIdAsync(int id)
        {
            var p = await _programRepository.GetByIdAsync(id);
            if (p == null)
                return new ResponseDTO<GetProgramDTO>
                {
                    Status_code= 404,
                    Data = null
                };

            var data = new GetProgramDTO
            {
                Program_Id = p.Program_Id,
                Name = p.Name,
                Description = p.Description
            };

            return new ResponseDTO<GetProgramDTO> { Status_code = 200, Data = data };
        }

        public async Task<ResponseDTO<GetProgramDTO>> AddAsync(AddProgramDTO dto)
        {
            var program = new Program_
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _programRepository.AddAsync(program);

            var data = new GetProgramDTO
            {
                Program_Id = program.Program_Id,
                Name = program.Name,
                Description = program.Description
            };

            return new ResponseDTO<GetProgramDTO>
            {
                Status_code= 200,
                Data = data
            };
        }

        public async Task<ResponseDTO<GetProgramDTO>> UpdateAsync(int id, AddProgramDTO dto)
        {
            var existing = await _programRepository.GetByIdAsync(id);
            if (existing == null)
                return new ResponseDTO<GetProgramDTO>
                {
                    Status_code = 404,
                    Data = null
                };

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _programRepository.UpdateAsync(existing);

            var data = new GetProgramDTO
            {
                Program_Id = existing.Program_Id,
                Name = existing.Name,
                Description = existing.Description
            };

            return new ResponseDTO<GetProgramDTO>
            {
                Status_code= 200,
                Data = data
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _programRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _programRepository.DeleteAsync(existing);
            return true;
        }
    }
}