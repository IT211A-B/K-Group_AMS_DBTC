using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IPermissionService
    {
        Task<ResponseDTO<IEnumerable<GetPermissionDTO>>> GetAllAsync();
        Task<ResponseDTO<GetPermissionDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetPermissionDTO>> AddAsync(AddPermissionDTO dto);
        Task<ResponseDTO<GetPermissionDTO>> UpdateAsync(int id, AddPermissionDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}