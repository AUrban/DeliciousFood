using DeliciousFood.Api.Security.Options;
using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Enumerations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;

namespace DeliciousFood.Api.Security.Implementation
{
    /// <summary>
    /// A provider implementation to get and create the logged user claims stored into access token. 
    /// Based on HttpContextAccessor and ClaimsPrincipal
    /// </summary>
    public class ClaimsPrincipalProvider : IClaimsPrincipalProvider
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        public ClaimsPrincipalProvider(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        public ClaimsIdentity ClaimsIdentity => User?.Identity as ClaimsIdentity;

        public ClaimsIdentity GenerateClaimsIdentity(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsAliases.UserIdAlias, user.Id.ToString()),
                new Claim(ClaimsAliases.UserPolicyMaskAlias, ((int)user.PolicyMask).ToString())
            };
            return new ClaimsIdentity(claims, ClaimsAliases.IdentityAuthenticationTypeAlias);
        }

        public int UserIdClaimValue =>
            (int.TryParse(User?.FindFirst(ClaimsAliases.UserIdAlias)?.Value, out int result) ? result : 0);

        public Policy PolicyMaskClaimValue =>
            (int.TryParse(User?.FindFirst(ClaimsAliases.UserPolicyMaskAlias)?.Value, out int result) ? (Policy)result : 0);


        // Similar to the call User property in the controller
        private ClaimsPrincipal User => HttpContextAccessor.HttpContext.User;
    }
}
