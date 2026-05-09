using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IRolePermissionService
    {
        Task<ResponseDTO<IEnumerable<GetRolePermissionDTO>>> GetAllAsync();
        Task<ResponseDTO<GetRolePermissionDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetRolePermissionDTO>> AddAsync(AddRolePermissionDTO dto);
        Task<ResponseDTO<GetRolePermissionDTO>> UpdateAsync(int id, AddRolePermissionDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}