using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Repositories;
using DeliciousFood.Services.Accounts.Model;
using DeliciousFood.Services.Exceptions;
using DeliciousFood.Services.Security;
using DeliciousFood.Services.Security.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeliciousFood.Services.Accounts.Implementation
{
    /// <summary>
    /// Account service implementation: login, logout, refresh access token operations
    /// </summary>
    public sealed class AccountService : IAccountService
    {
        /// <summary>
        /// Maximum number of refresh tokens for one user (for different devices)
        /// </summary>
        public const int MAX_REFRESH_TOKEN_COUNT = 5;


        private ITokenProvider TokenProvider { get; set; }

        private ISecurityProvider SecurityProvider { get; set; }

        private IEntityRepository<RefreshToken> RefreshTokenRepository { get; }

        private IEntityRepository<User> UserRepository { get; }


        public AccountService(ISecurityProvider securityProvider,
                              IEntityRepository<User> userRepository,
                              IEntityRepository<RefreshToken> refreshTokenRepository,
                              ITokenProvider tokenProvider)
        {
            SecurityProvider = securityProvider;
            UserRepository = userRepository;
            RefreshTokenRepository = refreshTokenRepository;
            TokenProvider = tokenProvider;
        }

        public async Task<TokenViewModel> LoginAsync(LoginViewModel model)
        {
            // getting user by name
            User user = UserRepository.UntrackedQuery.FirstOrDefault(x => x.Login == model.Login);
            if (user == null)
                throw new ServiceValidationException(nameof(User), "Invalid login or password!");

            // checking password
            if (!SecurityProvider.VerifyPassword(model.Password, user.PasswordHash))
                throw new ServiceValidationException(nameof(User), "Invalid login or password!");

            // access and update tokens are generated
            var accessToken = TokenProvider.GenerateToken(user, TokenType.AccessToken);
            var refreshToken = TokenProvider.GenerateToken(user, model.RememberMe ? TokenType.RefreshTokenRemember : TokenType.RefreshToken);

            var authRefreshTokenList = RefreshTokenRepository.UntrackedQuery.Where(e => e.UserId == user.Id);
            if (authRefreshTokenList.Count() > MAX_REFRESH_TOKEN_COUNT)
            {
                var authRefreshToken = authRefreshTokenList.OrderBy(e => e.CreateTime).FirstOrDefault();
                authRefreshToken = GetRefreshToken(authRefreshToken, refreshToken, user);
                await RefreshTokenRepository.UpdateAsync(authRefreshToken);
            }
            else
            {
                var authRefreshToken = RefreshTokenRepository.Create();
                authRefreshToken = GetRefreshToken(authRefreshToken, refreshToken, user);
                await RefreshTokenRepository.SaveAsync(authRefreshToken);
            }

            return new TokenViewModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return;

            var token = RefreshTokenRepository.Query.FirstOrDefault(e => e.Token == refreshToken);
            if (token == null)
                return;

            await RefreshTokenRepository.DeleteAsync(token);
        }

        public async Task<string> RefreshAsync(string refreshToken)
        {
            // if the refresh token was not passed, then we return the "not authorized" status (401)
            if (string.IsNullOrEmpty(refreshToken))
                throw new ServiceUnauthorizedException();

            // find the specified refresh token in the table with user tokens
            var token = RefreshTokenRepository.Query.FirstOrDefault(e => e.Token == refreshToken);
            if (token == null)
                throw new ServiceUnauthorizedException();

            // if the refresh token's lifetime has expired, return the "not authorized" status
            if (DateTime.UtcNow >= token.LifeTime)
            {
                await RefreshTokenRepository.DeleteAsync(token);
                throw new ServiceUnauthorizedException();
            }

            // return a new access token
            return TokenProvider.GenerateToken(token.User, TokenType.AccessToken);
        }

        // Generate a new refresh token
        private RefreshToken GetRefreshToken(RefreshToken authRefreshToken, string refreshToken, User user)
        {
            authRefreshToken.Token = refreshToken;
            authRefreshToken.UserId = user.Id;
            authRefreshToken.CreateTime = DateTime.UtcNow;
            authRefreshToken.LifeTime = TokenProvider.ParseToken(refreshToken).ValidTo;
            return authRefreshToken;
        }
    }
}
