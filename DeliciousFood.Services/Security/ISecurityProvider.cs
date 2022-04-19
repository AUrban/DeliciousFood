namespace DeliciousFood.Services.Security
{
    /// <summary>
    /// Interface for security operations with user password
    /// Can be implemented in various ways: Custom provider, Core.Identity, ...
    /// </summary>
    public interface ISecurityProvider
    {
        /// <summary>
        /// Verify user password and given password
        /// </summary>
        public bool VerifyPassword(string userName, string password);

        /// <summary>
        /// Set a new user password
        /// </summary>
        public string GetHashedPassword(string password);

        /// <summary>
        /// Password complexity check
        /// </summary>
        public string CheckPasswordComplexity(string passsword);
    }
}
