using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IUserService
    {
        Task<ResponseDTO<IEnumerable<GetUserDTO>>> GetAllAsync();
        Task<ResponseDTO<GetUserDTO>> GetByIdAsync(int id);
        Task<RegisterDTO> AddAsync(AddUserDTO userDto);
        //Task<GetUserDTO?> UpdateAsync(int id, AddUserDTO userDto);
        Task<bool> DeleteAsync(int id);
    }
}