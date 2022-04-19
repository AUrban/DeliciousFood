using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Repositories;
using DeliciousFood.Services.Accounts.Implementation;
using DeliciousFood.Services.Accounts.Model;
using DeliciousFood.Services.Exceptions;
using DeliciousFood.Services.Security;
using DeliciousFood.Services.Security.Enums;
using DeliciousFood.Tests.UnitTests.Helpers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeliciousFood.Tests.UnitTests.Services
{
    public class AccountServiceTest : BaseServiceTest
    {
        public AccountServiceTest()
        {
        }

        #region Login

        [Fact]
        public async Task LoginAsyncInvalidTest()
        {
            // arrange
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var refreshTokenRepository = Substitute.For<IEntityRepository<RefreshToken>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var tokenProvider = Substitute.For<ITokenProvider>();
            var service = new AccountService(securityProvider, userRepository, refreshTokenRepository, tokenProvider);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            var query = userList.AsQueryable();
            userRepository.UntrackedQuery.Returns(query);
            securityProvider.VerifyPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(args => args.ArgAt<string>(0) == args.ArgAt<string>(1));

            var loginViewModel = new LoginViewModel
            {
                Login = userList[2].Login,
                Password = "bad password"
            };

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await service.LoginAsync(loginViewModel));
            securityProvider.Received(1).VerifyPassword(loginViewModel.Password, userList[2].PasswordHash);
        }

        [Fact]
        public async Task LoginAsyncValidTest()
        {
            // arrange
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var refreshTokenRepository = Substitute.For<IEntityRepository<RefreshToken>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var tokenProvider = Substitute.For<ITokenProvider>();
            var service = new AccountService(securityProvider, userRepository, refreshTokenRepository, tokenProvider);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            var query = userList.AsQueryable();
            userRepository.UntrackedQuery.Returns(query);
            securityProvider.VerifyPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(args => args.ArgAt<string>(0) == args.ArgAt<string>(1));

            var user = userList[2];
            var loginViewModel = new LoginViewModel
            {
                Login = user.Login,
                Password = user.PasswordHash
            };
            tokenProvider.GenerateToken(user, TokenType.AccessToken).Returns("Access token");
            tokenProvider.GenerateToken(user, TokenType.RefreshToken).Returns("Refresh token");
            
            refreshTokenRepository.UntrackedQuery.Returns(new List<RefreshToken>().AsQueryable());
            refreshTokenRepository.Create().Returns(new RefreshToken());

            // act 
            var tokenViewModel = await service.LoginAsync(loginViewModel);

            // assert
            Assert.NotNull(tokenViewModel);
            Assert.Equal("Access token", tokenViewModel.AccessToken);
            Assert.Equal("Refresh token", tokenViewModel.RefreshToken);
            securityProvider.Received(1).VerifyPassword(loginViewModel.Password, user.PasswordHash);
            tokenProvider.ReceivedWithAnyArgs(2).GenerateToken(default, default);
            refreshTokenRepository.Received(1).Create();
            await refreshTokenRepository.ReceivedWithAnyArgs(1).SaveAsync(default);
        }

        #endregion // Login

        #region Refresh

        [Fact]
        public async Task RefreshAsyncInvalidTest()
        {
            // arrange
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var refreshTokenRepository = Substitute.For<IEntityRepository<RefreshToken>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var tokenProvider = Substitute.For<ITokenProvider>();
            var service = new AccountService(securityProvider, userRepository, refreshTokenRepository, tokenProvider);

            var refreshToken = "refreshToken1";
            var refreshTokenList = new List<RefreshToken>
            {
                new RefreshToken
                {
                    Id = 1,
                    UserId = 1,
                    Token = "refreshToken1",
                    CreateTime = DateTime.UtcNow.AddMinutes(-10),
                    LifeTime = DateTime.UtcNow
                },
                new RefreshToken
                {
                    Id = 2,
                    UserId = 2,
                    Token = "refreshToken2",
                    CreateTime = DateTime.UtcNow.AddMinutes(-10),
                    LifeTime = DateTime.UtcNow.AddMinutes(10)
                }
            };
            var query = refreshTokenList.AsQueryable();
            refreshTokenRepository.Query.Returns(query);

            // act and assert
            await Assert.ThrowsAsync<ServiceUnauthorizedException>(async () => await service.RefreshAsync(refreshToken));
            await refreshTokenRepository.ReceivedWithAnyArgs(1).DeleteAsync(default);
        }

        [Fact]
        public async Task RefreshAsyncValidTest()
        {
            // arrange
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var refreshTokenRepository = Substitute.For<IEntityRepository<RefreshToken>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var tokenProvider = Substitute.For<ITokenProvider>();
            var service = new AccountService(securityProvider, userRepository, refreshTokenRepository, tokenProvider);

            var refreshToken = "refreshToken2";
            var refreshTokenList = new List<RefreshToken>
            {
                new RefreshToken
                {
                    Id = 1,
                    UserId = 1,
                    Token = "refreshToken1",
                    CreateTime = DateTime.UtcNow.AddMinutes(-10),
                    LifeTime = DateTime.UtcNow
                },
                new RefreshToken
                {
                    Id = 2,
                    UserId = 2,
                    Token = "refreshToken2",
                    CreateTime = DateTime.UtcNow.AddMinutes(-10),
                    LifeTime = DateTime.UtcNow.AddMinutes(10)
                }
            };
            var query = refreshTokenList.AsQueryable();
            refreshTokenRepository.Query.Returns(query);
            tokenProvider.GenerateToken(Arg.Any<User>(), TokenType.AccessToken).Returns("Access token");

            // act
            var accessToken = await service.RefreshAsync(refreshToken);

            // assert
            Assert.NotNull(accessToken);
            Assert.Equal("Access token", accessToken);
            tokenProvider.ReceivedWithAnyArgs(1).GenerateToken(default, default);
        }

        #endregion // Refresh

        #region Logout

        [Fact]
        public async Task LogoutAsyncValidTest()
        {
            // arrange
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var refreshTokenRepository = Substitute.For<IEntityRepository<RefreshToken>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var tokenProvider = Substitute.For<ITokenProvider>();
            var service = new AccountService(securityProvider, userRepository, refreshTokenRepository, tokenProvider);

            var refreshToken = "refreshToken1";
            var refreshTokenList = new List<RefreshToken>
            {
                new RefreshToken
                {
                    Id = 1,
                    UserId = 1,
                    Token = "refreshToken1",
                    CreateTime = DateTime.UtcNow.AddMinutes(-10),
                    LifeTime = DateTime.UtcNow
                },
                new RefreshToken
                {
                    Id = 2,
                    UserId = 2,
                    Token = "refreshToken2",
                    CreateTime = DateTime.UtcNow.AddMinutes(-10),
                    LifeTime = DateTime.UtcNow.AddMinutes(10)
                }
            };
            var query = refreshTokenList.AsQueryable();
            refreshTokenRepository.Query.Returns(query);

            // act
            await service.LogoutAsync(refreshToken);

            // assert
            await refreshTokenRepository.ReceivedWithAnyArgs(1).DeleteAsync(default);
        }

        #endregion // Logout
    }
}
