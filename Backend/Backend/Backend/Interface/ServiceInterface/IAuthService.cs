using Backend.Backend.DTOs;

namespace Backend.Backend.Interface.ServiceInterface
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginUserDto login);
    }
}
