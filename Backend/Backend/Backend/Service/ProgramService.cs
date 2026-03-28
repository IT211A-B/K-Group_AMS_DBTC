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

        public async Task<IEnumerable<GetProgramDTO>> GetAllAsync()
        {
            var programs = await _programRepository.GetAllAsync();
            return programs.Select(p => new GetProgramDTO
            {
                Program_Id = p.Program_Id,
                Name = p.Name,
                Description = p.Description
            });
        }

        public async Task<GetProgramDTO?> GetByIdAsync(int id)
        {
            var p = await _programRepository.GetByIdAsync(id);
            if (p == null) return null;

            return new GetProgramDTO
            {
                Program_Id = p.Program_Id,
                Name = p.Name,
                Description = p.Description
            };
        }

        public async Task<GetProgramDTO> AddAsync(AddProgramDTO dto)
        {
            var program = new Program_
            {
                Name = dto.Name,
                Description = dto.Description
            };

            await _programRepository.AddAsync(program);

            return new GetProgramDTO
            {
                Program_Id = program.Program_Id,
                Name = program.Name,
                Description = program.Description
            };
        }

        public async Task<GetProgramDTO?> UpdateAsync(int id, AddProgramDTO dto)
        {
            var existing = await _programRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = dto.Name;
            existing.Description = dto.Description;

            await _programRepository.UpdateAsync(existing);

            return new GetProgramDTO
            {
                Program_Id = existing.Program_Id,
                Name = existing.Name,
                Description = existing.Description
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