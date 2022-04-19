using DeliciousFood.Api.Controllers;
using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.Services.Accounts;
using DeliciousFood.Services.Accounts.Model;
using DeliciousFood.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Threading.Tasks;
using Xunit;

namespace DeliciousFood.Tests.UnitTests.Controllers
{
    public class AccountControllerTest
    {
        public AccountControllerTest()
        {
        }

        #region Login

        [Fact]
        public async Task LoginAsyncInvalidLoginTest()
        {
            // arrange
            var service = Substitute.For<IAccountService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new AccountsController(loggerFactory, service);

            var loginViewModel = new LoginViewModel
            {
                Login = "bad login",
                Password = "password"
            };
            service.LoginAsync(Arg.Is<LoginViewModel>(x => x.Login == "bad login")).Throws(new ServiceNotFoundException(typeof(User)));

            // act and assert
            await Assert.ThrowsAsync<ServiceNotFoundException>(async () => await controller.LoginAsync(loginViewModel));
            await service.Received(1).LoginAsync(loginViewModel);
        }

        [Fact]
        public async Task LoginAsyncInvalidPasswordTest()
        {
            // arrange
            var service = Substitute.For<IAccountService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new AccountsController(loggerFactory, service);

            var loginViewModel = new LoginViewModel
            {
                Login = "login",
                Password = "bad password"
            };
            service.LoginAsync(Arg.Is<LoginViewModel>(x => x.Password == "bad password")).Throws(new ServiceValidationException());

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.LoginAsync(loginViewModel));
            await service.Received(1).LoginAsync(loginViewModel);
        }

        [Fact]
        public async Task LoginAsyncValidTest()
        {
            // arrange
            var service = Substitute.For<IAccountService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new AccountsController(loggerFactory, service);

            var loginViewModel = new LoginViewModel
            {
                Login = "login",
                Password = "password"
            };
            service.LoginAsync(Arg.Is<LoginViewModel>(x => x.Login == "bad login")).Throws(new ServiceNotFoundException(typeof(User)));
            service.LoginAsync(Arg.Is<LoginViewModel>(x => x.Password == "bad password")).Throws(new ServiceValidationException());
            var expectedTokenViewModel = new TokenViewModel
            {
                AccessToken = "AccessToken",
                RefreshToken = "RefreshToken"
            };
            service.LoginAsync(Arg.Is<LoginViewModel>(x => x.Login != "bad login" && x.Password != "bad password")).Returns(expectedTokenViewModel);

            // act
            var actionResult = await controller.LoginAsync(loginViewModel);

            // assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualTokenViewModel = (TokenViewModel)okResult.Value;
            Assert.Equal(expectedTokenViewModel.AccessToken, actualTokenViewModel.AccessToken);
            Assert.Equal(expectedTokenViewModel.RefreshToken, actualTokenViewModel.RefreshToken);
            await service.Received(1).LoginAsync(loginViewModel);
        }

        #endregion // Login

        #region Logout

        [Fact]
        public async Task LogoutAsyncTokenTest()
        {
            // arrange
            var service = Substitute.For<IAccountService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new AccountsController(loggerFactory, service);

            var refreshToken = "refresh token";

            // act
            var actionResult = await controller.LogoutAsync(refreshToken);

            // assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkResult>(actionResult);
            Assert.Equal(StatusCodes.Status200OK, (actionResult as OkResult).StatusCode);
            await service.Received(1).LogoutAsync(refreshToken);
        }

        #endregion // Logout

        #region Refresh

        [Fact]
        public async Task RefreshAsyncInvalidTokenTest()
        {
            // arrange
            var service = Substitute.For<IAccountService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new AccountsController(loggerFactory, service);

            var refreshToken = "bad refresh token";
            service.RefreshAsync(refreshToken).Throws(new ServiceUnauthorizedException());

            // act and assert
            await Assert.ThrowsAsync<ServiceUnauthorizedException>(async () => await controller.RefreshAsync(refreshToken));
            await service.Received(1).RefreshAsync(refreshToken);
        }

        [Fact]
        public async Task RefreshAsyncValidTokenTest()
        {
            // arrange
            var service = Substitute.For<IAccountService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new AccountsController(loggerFactory, service);

            var badRefreshToken = "bad refresh token";
            var refreshToken = "refresh token";
            var accessToken = "access token";
            service.RefreshAsync(badRefreshToken).Throws(new ServiceUnauthorizedException());
            service.RefreshAsync(Arg.Is<string>(x => x != badRefreshToken)).Returns(accessToken);

            // act
            var actionResult = await controller.RefreshAsync(refreshToken);

            // assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Equal(accessToken, okResult.Value);
            await service.Received(1).RefreshAsync(refreshToken);
        }

        #endregion // Refresh
    }
}
