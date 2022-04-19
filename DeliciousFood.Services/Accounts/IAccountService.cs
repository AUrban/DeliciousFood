using DeliciousFood.Services.Accounts.Model;
using DeliciousFood.Services.Base;
using System.Threading.Tasks;

namespace DeliciousFood.Services.Accounts
{
    /// <summary>
    /// A service to user login
    /// </summary>
    public interface IAccountService : IService
    {
        /// <summary>
        /// User login to the system.
        /// Creates a "session" (tokens)
        /// </summary>
        Task<TokenViewModel> LoginAsync(LoginViewModel model);

        /// <summary>
        /// User logout
        /// Close the "session"
        /// </summary>
        Task LogoutAsync(string refreshToken);

        /// <summary>
        /// Called from the client when the access token has expired
        /// A new access-token is returned, and there is no need to log in again
        /// </summary>
        Task<string> RefreshAsync(string refreshToken);
    }
}
