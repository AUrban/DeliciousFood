using DeliciousFood.DataAccess.Enumerations;

namespace DeliciousFood.Services.Security
{
    /// <summary>
    /// A provider to validate logged user permissions
    /// </summary>
    public interface IPolicyValidator
    {
        /// <summary>
        /// Existing compliance checking two policies
        /// </summary>
        bool ValidatePolicyIntersect(Policy policyLeft, Policy policyRight);

        /// <summary>
        /// Existing compliance checking logged user policy and given permission
        /// </summary>
        bool ValidateUserPolicyIntersect(Policy policy);
    }
}
