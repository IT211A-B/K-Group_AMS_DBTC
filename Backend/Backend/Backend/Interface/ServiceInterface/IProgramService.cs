using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IProgramService
    {
        Task<IEnumerable<GetProgramDTO>> GetAllAsync();
        Task<GetProgramDTO?> GetByIdAsync(int id);
        Task<GetProgramDTO> AddAsync(AddProgramDTO dto);
        Task<GetProgramDTO?> UpdateAsync(int id, AddProgramDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}