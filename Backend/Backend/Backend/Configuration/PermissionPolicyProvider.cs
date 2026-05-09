using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace Backend.Backend.Configuration
{
    /// <summary>
    /// Custom policy provider that dynamically creates authorization policies
    /// based on permission names
    /// </summary>
    /// <remarks>
    /// Instead of predefining all policies in Startup/Program,
    /// this provider generates policies at runtime using the policy name
    /// 
    /// Example:
    /// [Authorize(Policy = "CreateUser")]
    /// → Automatically creates a policy requiring "CreateUser" permission
    /// </remarks>
    // class = policy provider
    public class PermissionPolicyProvider
        : DefaultAuthorizationPolicyProvider
    {
        /// <summary>
        /// Constructor for PermissionPolicyProvider
        /// </summary>
        /// <param name="options">
        /// Authorization options used by the base policy provider
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when options is null
        /// </exception>
        // constructor
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
            : base(options ?? throw new ArgumentNullException(nameof(options)))
        {
        }

        /// <summary>
        /// Retrieves or dynamically creates an authorization policy based on the policy name
        /// </summary>
        /// <param name="policyName">
        /// The name of the policy, typically representing a permission
        /// </param>
        /// <returns>
        /// An <see cref="AuthorizationPolicy"/> that requires the specified permission
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when policyName is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when policyName is empty or whitespace
        /// </exception>
        /// <remarks>
        /// Process flow:
        /// 1. Validate policy name
        /// 2. Create a policy builder
        /// 3. Attach PermissionRequirement using policy name
        /// 4. Build and return the policy
        /// </remarks>
        public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Validates that the policy name is not null
            if (policyName == null)
                throw new ArgumentNullException(nameof(policyName));

            // Validates that the policy name is not empty or whitespace
            if (string.IsNullOrWhiteSpace(policyName))
                throw new ArgumentException("Policy name cannot be empty.", nameof(policyName));

            // Builds a new authorization policy dynamically
            var policy = new AuthorizationPolicyBuilder();

            // Adds a permission requirement based on the policy name
            policy.AddRequirements(new PermissionRequirement(policyName));

            // Returns the constructed authorization policy
            return Task.FromResult<AuthorizationPolicy?>(policy.Build());
        }
    }
}