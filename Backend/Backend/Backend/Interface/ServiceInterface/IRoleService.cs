using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IRoleService
    {
        Task<ResponseDTO<IEnumerable<GetRoleDTO>>> GetAllAsync();
        Task<ResponseDTO<GetRoleDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetRoleDTO>> AddAsync(AddRoleDTO dto);
        Task<ResponseDTO<GetRoleDTO>> UpdateAsync(int id, AddRoleDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}