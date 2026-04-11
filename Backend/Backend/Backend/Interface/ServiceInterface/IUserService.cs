using Backend.Backend.DTO;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IUserService
    {
        Task<IEnumerable<GetUserDTO>> GetAllAsync();
        Task<GetUserDTO?> GetByIdAsync(int id);
        Task<UserResponse> AddAsync(AddUserDTO userDto);
        Task<LoginResult> LoginAsync(LoginUserDto login);
        //Task<GetUserDTO?> UpdateAsync(int id, AddUserDTO userDto);
        Task<bool> DeleteAsync(int id);
    }
}