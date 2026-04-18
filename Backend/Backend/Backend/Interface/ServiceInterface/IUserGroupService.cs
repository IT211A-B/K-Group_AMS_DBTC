using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IUserGroupService
    {
        Task<ResponseDTO<IEnumerable<GetUserGroupDTO>>> GetAllAsync();
        Task<ResponseDTO<GetUserGroupDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetUserGroupDTO>> AddAsync(AddUserGroupDTO dto);
        Task<ResponseDTO<GetUserGroupDTO>> UpdateAsync(int id, AddUserGroupDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}