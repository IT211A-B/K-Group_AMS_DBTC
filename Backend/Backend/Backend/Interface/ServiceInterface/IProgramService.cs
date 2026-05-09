using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IProgramService
    {
        Task<ResponseDTO<IEnumerable<GetProgramDTO>>> GetAllAsync();
        Task<ResponseDTO<GetProgramDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetProgramDTO>> AddAsync(AddProgramDTO dto);
        Task<ResponseDTO<GetProgramDTO>> UpdateAsync(int id, AddProgramDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}