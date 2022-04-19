namespace DeliciousFood.Api.Security.Options
{
    /// <summary>
    /// Claims aliases for the logged user parameters values stored into access token
    /// </summary>
    public class ClaimsAliases
    {
        /// <summary>
        /// Auth type alias
        /// </summary>
        public const string IdentityAuthenticationTypeAlias = "JwtToken";

        /// <summary>
        /// User identifier alias
        /// </summary>
        public const string UserIdAlias = "UserId";

        /// <summary>
        /// User policies alias
        /// </summary>
        public const string UserPolicyMaskAlias = "UserPolicyMask";
    }
}
