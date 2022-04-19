namespace DeliciousFood.Services.Security.Enums
{
    /// <summary>
    /// Token lifetime type enumeration
    /// </summary>
    public enum TokenType
    {
        /// <summary>
        /// Access token lives for a very limited time
        /// Changed by means of refresh token
        /// </summary>
        AccessToken = 0,

        /// <summary>
        /// Refresh token lives longer
        /// </summary>
        RefreshToken,

        /// <summary>
        /// Refresh token with the "remember me" mode lives even longer
        /// </summary>
        RefreshTokenRemember
    }
}
