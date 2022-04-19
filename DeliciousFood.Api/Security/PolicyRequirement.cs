using DeliciousFood.DataAccess.Enumerations;
using Microsoft.AspNetCore.Authorization;

namespace DeliciousFood.Api.Security
{
    /// <summary>
    /// Logged user permission requirement
    /// </summary>
    public class PolicyRequirement : IAuthorizationRequirement
    {
        public Policy PolicyMask { get; }

        public PolicyRequirement(Policy policyMask)
        {
            PolicyMask = policyMask;
        }
    }
}
