using Backend.Backend.Interface.ConfigureInterface;
using Backend.Backend.Model;
using Microsoft.AspNetCore.Identity;

namespace Backend.Backend.Configuration
{
    /// <summary>
    /// Centralizes password hashing using ASP.NET Core Identity's PBKDF2 hasher.
    /// </summary>
    public class PasswordHashingService : IPasswordHashingService
    {
        private readonly PasswordHasher<User> _hasher = new();

        public string HashPassword(User user, string password)
            => _hasher.HashPassword(user, password);

        public bool VerifyPassword(User user, string hashedPassword, string providedPassword)
            => _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword)
                != PasswordVerificationResult.Failed;
    }
}
