using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Security;

namespace DeliciousFood.Services.Security.Implementation
{
    /// <summary>
    /// A provider implementation to validate logged user permissions
    /// </summary>
    public class PolicyValidator : IPolicyValidator
    {
        public IUserSessionProvider UserSessionProvider { get; set; }

        public PolicyValidator(IUserSessionProvider userSessionProvider)
        {
            UserSessionProvider = userSessionProvider;
        }

        public bool ValidatePolicyIntersect(Policy policyMaskLeft, Policy policyMaskRight)
        {
            return (policyMaskLeft & policyMaskRight) != Policy.None;
        }

        public bool ValidateUserPolicyIntersect(Policy policyMask)
        {
            return ValidatePolicyIntersect(UserSessionProvider.PolicyMask, policyMask);
        }
    }
}
