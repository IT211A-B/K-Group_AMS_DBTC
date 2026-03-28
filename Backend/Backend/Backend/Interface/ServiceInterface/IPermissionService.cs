using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IPermissionService
    {
        Task<IEnumerable<GetPermissionDTO>> GetAllAsync();
        Task<GetPermissionDTO?> GetByIdAsync(int id);
        Task<GetPermissionDTO> AddAsync(AddPermissionDTO dto);
        Task<GetPermissionDTO?> UpdateAsync(int id, AddPermissionDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}