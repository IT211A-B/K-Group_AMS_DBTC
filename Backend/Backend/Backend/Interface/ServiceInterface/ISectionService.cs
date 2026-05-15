using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface ISectionService
    {
        Task<ResponseDTO<IEnumerable<GetSectionDTO>>> GetAllAsync();
        Task<ResponseDTO<GetSectionDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetSectionDTO>> AddAsync(AddSectionDTO dto);
        Task<ResponseDTO<GetSectionDTO>> UpdateAsync(int id, AddSectionDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}