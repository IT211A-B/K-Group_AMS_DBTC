using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IUserGroupService
    {
        Task<IEnumerable<GetUserGroupDTO>> GetAllAsync();
        Task<GetUserGroupDTO?> GetByIdAsync(int id);
        Task<GetUserGroupDTO> AddAsync(AddUserGroupDTO dto);
        Task<GetUserGroupDTO?> UpdateAsync(int id, AddUserGroupDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}