using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Backend.Backend.Helper;
using Backend.Backend.Model;
using System.ComponentModel;
using Backend.Backend.Helper.Enum;
using Microsoft.EntityFrameworkCore;
using Backend.Backend.Interface.ConfigureInterface;

namespace Backend.Backend.Configuration
{
    /// <summary>
    /// Service responsible for building user claims dynamically.
    /// </summary>
    /// <remarks>
    /// This service retrieves the user's roles and permissions from the database
    /// and converts them into claims to be embedded inside a JWT.
    /// </remarks>
    public class ClaimService : IClaimService
    {
        /// <summary>
        /// Manages user-related operations such as retrieving roles.
        /// </summary>
        private readonly UserManager<User> _userManager;

        /// <summary>
        /// Database context used to access roles and permissions.
        /// </summary>
        private readonly DatabaseLibrary _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimService"/> class.
        /// </summary>
        /// <param name="userManager">
        /// Provides access to user management operations such as retrieving roles.
        /// </param>
        /// <param name="context">
        /// The database context used to query roles and permissions.
        /// </param>
        public ClaimService(UserManager<User> userManager,
                            DatabaseLibrary context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Builds a list of claims for the specified user.
        /// </summary>
        /// <param name="user">
        /// The authenticated user for whom claims will be generated.
        /// </param>
        /// <returns>
        /// A list of <see cref="Claim"/> objects containing user identity,
        /// roles, and permissions.
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Adds basic identity claims (e.g., username)
        /// 2. Retrieves user roles from ASP.NET Identity
        /// 3. Fetches permissions associated with each role
        /// 4. Converts roles and permissions into claims
        /// 
        /// These claims are later used for authorization via policies.
        /// </remarks>
        public async Task<List<Claim>> GetClaimsAsync(User user)
        {
            // Collection that will store all generated claims.
            var claims = new List<Claim>();

            // Adds the user's username as a claim.
            // ClaimTypes.Name represents the user's identity name.
            claims.Add(new Claim(ClaimTypes.Name, user.Full_Name));

            // Retrieves all roles assigned to the user.
            var roles = await _userManager.GetRolesAsync(user);

            // Iterates through each role assigned to the user.
            foreach (var role in roles)
            {
                // Adds the role as a claim.
                claims.Add(new Claim(ClaimTypes.Role, role));

                // Retrieves the role entity from the database based on role name.
                var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Name == role);

                //Check if null
                if (roleEntity == null) continue;

                // Retrieves all permissions linked to the current role.
                // This queries the RolePermissions table and selects permission names.
                var permissions = await _context.RolePermissions
                    .Where(rp => rp.Role_ID == roleEntity.Id)
                    .Select(rp => rp.Permission_ID)
                    .ToListAsync();

                // Iterates through each permission and adds it as a claim.
                foreach (var permission in permissions)
                {
                    // Adds a custom "Permission" claim.
                    // This is used for policy-based authorization (e.g., "User.Delete").
                    claims.Add(new Claim("Permission", permission.ToString()));
                }
            }

            return claims;
        }
    }
}
