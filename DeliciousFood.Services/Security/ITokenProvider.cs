using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.Services.Security.Enums;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace DeliciousFood.Services.Security
{
    /// <summary>
    /// A provider for working with auth token: to validate given token and generate it
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Generate the auth token for a given user and token type
        /// </summary>
        string GenerateToken(User user, TokenType type);

        /// <summary>
        /// Convert token string representation to token
        /// </summary>
        SecurityToken ParseToken(string token);

        /// <summary>
        /// Validation token and retrieving claims identity (User)
        /// </summary>
        public ClaimsPrincipal ValidateToken(string token);
    }
}
