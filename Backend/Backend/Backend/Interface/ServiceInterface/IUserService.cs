using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IUserService
    {
        Task<ResponseDTO<IEnumerable<GetUserDTO>>> GetAllAsync();
        Task<ResponseDTO<GetUserDTO>> GetByIdAsync(int id);
        Task<ResponseDTO<GetUserDTO>> AddAsync(AddUserDTO userDto);
        Task<LoginResult> LoginAsync(LoginUserDto login);
        //Task<GetUserDTO?> UpdateAsync(int id, AddUserDTO userDto);
        Task<bool> DeleteAsync(int id);
    }
}