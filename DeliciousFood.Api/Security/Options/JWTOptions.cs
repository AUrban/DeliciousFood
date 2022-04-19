using DeliciousFood.Services.Security.Enums;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace DeliciousFood.Api.Security.Options
{
    /// <summary>
    /// Settings for working with JWT token
    /// </summary>
    public static class JWTOptions
    {
        /// <summary>
        /// Token issuer
        /// </summary>
        public const string ISSUER = "IssuerCompany";

        /// <summary>
        /// Token consumer
        /// </summary>
        public const string AUDIENCE = "AudienceCompany";

        /// <summary>
        /// Security algorithm
        /// </summary>
        public const string ALGO = SecurityAlgorithms.HmacSha512Signature;

        /// <summary>
        /// Time skew
        /// </summary>
        public readonly static TimeSpan SKEW = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Calculating expire date for token
        /// </summary>
        private const int ACCESS_TOKEN_LIFE_TIME_IN_MINUTES = 15;
        private const int REFRESH_TOKEN_LIFE_TIME_IN_MINUTES = 30;
        private const int REFRESH_TOKEN_REMEMBER_LIFE_TIME_IN_MINUTES = 24 * 60;
        public static DateTime GetExpiresTime(DateTime startTokenTime, TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.AccessToken:
                    return startTokenTime.AddMinutes(JWTOptions.ACCESS_TOKEN_LIFE_TIME_IN_MINUTES);
                case TokenType.RefreshToken:
                    return startTokenTime.AddMinutes(JWTOptions.REFRESH_TOKEN_LIFE_TIME_IN_MINUTES);
                case TokenType.RefreshTokenRemember:
                    return startTokenTime.AddMinutes(JWTOptions.REFRESH_TOKEN_REMEMBER_LIFE_TIME_IN_MINUTES);
                default:
                    throw new ArgumentException("This type of token is not supported!");
            }
        }

        /// <summary>
        /// Symmetric security key to encrypt the token
        /// </summary>
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }

        /// <summary>
        /// Getting token validation parameters when receiving a request
        /// </summary>
        public static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, // the security key is validated
                IssuerSigningKey = GetSymmetricSecurityKey(), // security key

                ValidateIssuer = true, // the issuer token is validated
                ValidIssuer = ISSUER,

                ValidateAudience = true, // the consumer token is validated
                ValidAudience = AUDIENCE,

                ValidateLifetime = true, // the token lifetime is validated

                ClockSkew = SKEW // skew for the access token lifetime
            };
        }

        /// <summary>
        /// Private 512-bit token encryption key
        /// </summary>
        private const string KEY = "z4dn1TksgIIN4ZPH9t7vbA+S+zOGXVkLLVvQeLWHlcDkZw3T/YO6RS5iI85te6zL8v+NXOBmWwNyVZN9LWX9LOPMsVX3rYzZhcm+TKZlZtRdYWtTJMUMR221+TMnvjSsAc1O4Pj6rEDLygI0Q+b/Kc5MbUbGi4uGFKSO+BXknV4=";
    }
}
