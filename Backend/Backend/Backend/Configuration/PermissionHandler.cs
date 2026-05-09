using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Backend.Configuration
{
    /// <summary>
    /// Authorization handler that validates whether a user has a required permission.
    /// </summary>
    /// <remarks>
    /// This handler is part of a policy-based authorization system.
    /// It checks if the authenticated user contains a specific "Permission" claim.
    /// If the claim exists and matches the required permission, access is granted.
    /// </remarks>
    // class = authorization handler
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        /// <summary>
        /// Handles the authorization logic for permission-based access control.
        /// </summary>
        /// <param name="context">
        /// Contains information about the current user and their claims.
        /// </param>
        /// <param name="requirement">
        /// The required permission that must be satisfied for authorization.
        /// </param>
        /// <returns>
        /// A completed task. Authorization succeeds if the user has the required permission.
        /// </returns>
        /// <remarks>
        /// Process flow:
        /// 1. Check if the user has a "Permission" claim.
        /// 2. Compare the claim value with the required permission.
        /// 3. If matched, call context.Succeed() to grant access.
        /// </remarks>
        // method = authorization logic
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            // Checks if the current user has the required permission claim.
            if (context.User.HasClaim("Permission", requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
