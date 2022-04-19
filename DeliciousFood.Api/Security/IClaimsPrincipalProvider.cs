using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Enumerations;
using System.Security.Claims;

namespace DeliciousFood.Api.Security
{
    /// <summary>
    /// A provider interface to get and create the logged user claims
    /// </summary>
    public interface IClaimsPrincipalProvider
    {
        /// <summary>
        /// Logged user wrapped to Identity with claims
        /// </summary>
        ClaimsIdentity ClaimsIdentity { get; }

        /// <summary>
        /// Generate user parameters wrapped into claims
        /// </summary>
        ClaimsIdentity GenerateClaimsIdentity(User user);

        /// <summary>
        /// Getting logged user id value from claims
        /// </summary>
        int UserIdClaimValue { get; }

        /// <summary>
        /// Getting logged user policies value from claims
        /// </summary>
        Policy PolicyMaskClaimValue { get; }
    }
}
