using DeliciousFood.Api.Security.Options;
using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.Services.Security;
using DeliciousFood.Services.Security.Enums;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DeliciousFood.Api.Security.Implementation
{
    /// <summary>
    /// A token provider implementation based on JWT tokens
    /// </summary>
    public class JWTTokenProvider : ITokenProvider
    {
        private IClaimsPrincipalProvider ClaimsPrincipalProvider { get; }

        public JWTTokenProvider(IClaimsPrincipalProvider claimsPrincipalProvider)
        {
            ClaimsPrincipalProvider = claimsPrincipalProvider;
        }

        public string GenerateToken(User user, TokenType tokenType)
        {
            var startTokenTime = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: JWTOptions.ISSUER,
                audience: JWTOptions.AUDIENCE,
                notBefore: startTokenTime, // from what moment the token starts working (stored in seconds)
                claims: ClaimsPrincipalProvider.GenerateClaimsIdentity(user).Claims, // user information is stored in the token
                expires: JWTOptions.GetExpiresTime(startTokenTime, tokenType),
                signingCredentials: new SigningCredentials(JWTOptions.GetSymmetricSecurityKey(), JWTOptions.ALGO)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public SecurityToken ParseToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("Token is not initialized!");
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenValidationParameters = JWTOptions.GetTokenValidationParameters();

            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal =
                tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            // if we did not receive a JwtSecurityToken or the encryption algorithm does not match the one used
            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(JWTOptions.ALGO, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return claimsPrincipal;
        }
    }
}
