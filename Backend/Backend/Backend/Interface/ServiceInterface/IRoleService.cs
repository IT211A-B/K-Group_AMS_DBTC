using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IRoleService
    {
        Task<IEnumerable<GetRoleDTO>> GetAllAsync();
        Task<GetRoleDTO?> GetByIdAsync(int id);
        Task<GetRoleDTO> AddAsync(AddRoleDTO dto);
        Task<GetRoleDTO?> UpdateAsync(int id, AddRoleDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}