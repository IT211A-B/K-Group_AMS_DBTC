using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IAccessService
    {
        Task<IEnumerable<GetAccessDTO>> GetAllAsync();
        Task<GetAccessDTO?> GetByIdAsync(int id);
        Task<GetAccessDTO> AddAsync(AddAccessDTO dto);
        Task<GetAccessDTO?> UpdateAsync(int id, AddAccessDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}