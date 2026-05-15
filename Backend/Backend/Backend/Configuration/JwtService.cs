using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Backend.Interface.ConfigureInterface;

namespace Backend.Backend.Configuration
{
    /// <summary>
    /// Service responsible for generating JSON Web Tokens (JWT).
    /// </summary>
    /// <remarks>
    /// JWT is used for authentication and authorization.
    /// This service creates a signed token containing user claims.
    /// </remarks>
    public class JwtService : IJwtService
    {
        /// <summary>
        /// Configuration instance used to access JWT settings from appsettings or environment variables.
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// Constructor for JwtService.
        /// </summary>
        /// <param name="config">
        /// Provides access to configuration values such as JWT Key, Issuer, Audience, and Expiry.
        /// </param>
        public JwtService(IConfiguration config)
        {
            _config = config;
        }
        /// <summary>
        /// Generates a signed JWT token using the provided claims.
        /// </summary>
        /// <param name="claims">
        /// A list of claims (e.g., user name, role, permissions) to be embedded inside the token.
        /// </param>
        /// <returns>
        /// A serialized JWT token string that can be used for authentication.
        /// </returns>
        /// <remarks>
        /// Steps:
        /// 1. Reads the secret key from configuration.
        /// 2. Creates a symmetric security key.
        /// 3. Generates signing credentials using HMAC SHA256.
        /// 4. Builds the JWT token with issuer, audience, claims, and expiration.
        /// 5. Serializes the token into a string.
        /// </remarks>
        public string GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:ExpireMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
