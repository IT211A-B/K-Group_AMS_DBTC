using Backend.Backend.Configuration;
using Backend.Backend.DTOs;
using Backend.Backend.Interface.ConfigureInterface;
using Backend.Backend.Interface.RepositoryInterface;
using Backend.Backend.Interface.ServiceInterface;
using Backend.Backend.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Backend.Backend.Model;

namespace Backend.Backend.Service
{
    /// <summary>
    /// Service responsible for handling authentication logic such as user login.
    /// </summary>
    /// <remarks>
    /// This service validates user credentials, retrieves claims, 
    /// and generates a JWT token upon successful authentication.
    /// </remarks>
    /// 

    public class AuthService(IUserRepository userRepository, IClaimService claimService, IJwtService jwtService, UserManager<User> userManager) : IAuthService // for primary constructor, will practice more on primary constructor as per release of C# 12
    {
        /// <summary>
        /// Repository used to access user data from the database.
        /// </summary>
        private readonly IUserRepository _userRepository  = userRepository;

        /// <summary>
        /// Service used to generate claims for the authenticated user.
        /// </summary>
        private readonly IClaimService _claimService = claimService;

        /// <summary>
        /// Service used to generate JWT tokens.
        /// </summary>
        private readonly IJwtService _jwtService = jwtService;

        // Since ASP Net already gave us User Manager, lets use the Identity System
        private readonly UserManager<User> _userManager = userManager;

        /// <summary>
        /// Authenticates a user using email/username and password.
        /// </summary>
        /// <param name="login">
        /// DTO containing login credentials (Email/Username and Password).
        /// </param>
        /// <returns>
        /// A <see cref="LoginResult"/> object indicating success or failure,
        /// along with a JWT token if authentication is successful.
        /// </returns>
        /// <remarks>
        /// Process flow:
        /// 1. Retrieve user by email or username.
        /// 2. Validate if user exists.
        /// 3. Verify password using Identity PBKDF2 hashing.
        /// 4. Generate claims for the user.
        /// 5. Generate JWT token using claims.
        /// 6. Return login result with token.
        /// </remarks>
        public async Task<LoginResult> LoginAsync(LoginUserDto login)
        {

            // method = find user by email
            var user = await _userManager.FindByEmailAsync(login.Email);

            // fallback = find by username
            if (user == null)
            {
                user = await _userManager.FindByNameAsync(login.Email);
            }

            // User Validation
            if (user == null) return new LoginResult
            {
                isSuccess = false,
                Token = null,
                Detail = "Email Incorrect   "
            };

            var passValidation = await _userManager.CheckPasswordAsync(user, login.Password);
            // Password validation
            if (!passValidation)
                return new LoginResult
                {
                    isSuccess = false,
                    Token = null,
                    Detail = "Password Incorrect"
                };

            // collect user's identity
            var claims = await _claimService.GetClaimsAsync(user);

            // generate token
            var token = _jwtService.GenerateToken(claims);


            return new LoginResult
            {
                isSuccess = true,
                Token = token,
                Detail = $"Welcome Back {user.Full_Name}"
            };
        }
    }
}
