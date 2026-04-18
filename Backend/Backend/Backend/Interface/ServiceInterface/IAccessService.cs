using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IAccessService
    {
        Task<ResponseDTO<IEnumerable<GetAccessDTO>>> GetAllAsync();
        Task<ResponseDTO<GetAccessDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetAccessDTO>> AddAsync(AddAccessDTO dto);
        Task<ResponseDTO<GetAccessDTO>> UpdateAsync(int id, AddAccessDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}