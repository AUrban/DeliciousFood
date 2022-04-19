using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Security;

namespace DeliciousFood.Api.Security.Implementation
{
    /// <summary>
    /// A provider implementation to get the current logged user parameters
    /// Based on token claims
    /// </summary>
    public class ClaimsUserSessionProvider : IUserSessionProvider
    {
        private IClaimsPrincipalProvider ClaimsPrincipalProvider { get; }

        public ClaimsUserSessionProvider(IClaimsPrincipalProvider claimsPrincipalProvider)
        {
            ClaimsPrincipalProvider = claimsPrincipalProvider;
        }

        public int UserId => ClaimsPrincipalProvider.UserIdClaimValue;

        public Policy PolicyMask => ClaimsPrincipalProvider.PolicyMaskClaimValue;
    }
}
