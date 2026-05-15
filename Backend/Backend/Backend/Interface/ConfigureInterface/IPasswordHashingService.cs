using Backend.Backend.Model;

namespace Backend.Backend.Interface.ConfigureInterface
{
    public interface IPasswordHashingService
    {
        string HashPassword(User user, string password);
        bool VerifyPassword(User user, string hashedPassword, string providedPassword);
    }
}
