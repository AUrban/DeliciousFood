using System.Text.RegularExpressions;
using BC = BCrypt.Net.BCrypt;

namespace DeliciousFood.Services.Security.Implementation
{
    /// <summary>
    /// Provider to enable security operations with user password
    /// </summary>
    public class SecurityProvider : ISecurityProvider
    {
        /// <summary>
        /// At least one upper case English letter, (?=.*?[A-Z])
        /// At least one lower case English letter, (?=.*?[a-z])
        /// At least one digit, (?=.*?[0-9])
        /// At least one special character, (?=.*?[#?!@$%^&*-])
        /// Minimum ten in length .{10,} (with the anchors)
        /// </summary>
        private readonly string PASSWORD_REGEX = @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$";
        private readonly string PASSWORD_ERROR =
            @"Password must have at least one upper case, one lower case, one digit, one special character, at least 6 characters!";


        public bool VerifyPassword(string password, string validPasswordHash)
        {
            return BC.Verify(password, validPasswordHash);
        }

        public string GetHashedPassword(string password)
        {
            return BC.HashPassword(password);
        }

        public string CheckPasswordComplexity(string passsword)
        {
            return new Regex(PASSWORD_REGEX).IsMatch(passsword) ? null : PASSWORD_ERROR;
        }
    }
}
