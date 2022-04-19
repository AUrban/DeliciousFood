using DeliciousFood.Services.Security;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace DeliciousFood.Api.Security
{
    /// <summary>
    /// Handler to verify PolicyRequirement.
    /// Checking that the logged user has required permissions
    /// </summary>
    public class PolicyHandler : AuthorizationHandler<PolicyRequirement>
    {
        private IPolicyValidator PolicyValidator { get; }

        public PolicyHandler(IPolicyValidator policyValidator)
        {
            PolicyValidator = policyValidator;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            if (PolicyValidator.ValidateUserPolicyIntersect(requirement.PolicyMask))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
