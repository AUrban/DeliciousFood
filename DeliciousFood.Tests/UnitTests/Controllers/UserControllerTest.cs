using DeliciousFood.Api.Controllers;
using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Base.Model;
using DeliciousFood.Services.Exceptions;
using DeliciousFood.Services.Users;
using DeliciousFood.Services.Users.Model;
using DeliciousFood.Tests.UnitTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DeliciousFood.Tests.UnitTests.Controllers
{
    /// <summary>
    /// Unit-tests for User controller methods
    /// </summary>
    public class UserControllerTest
    {
        public UserControllerTest()
        {
        }

        #region Get

        [Fact]
        public async Task GetAsyncNullableModelUnexpectedExceptionTest()
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            service.GetAsync(Arg.Is<FilterModel>(x => x == null)).Throws(new ArgumentNullException());

            // act and assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await controller.GetAsync(null));
            await service.Received(1).GetAsync(null);
        }

        [Fact]
        public async Task GetAsyncNullableModelServiceExceptionTest()
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            service.GetAsync(Arg.Is<FilterModel>(x => x == null)).Throws(new ServiceValidationException());

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.GetAsync(null));
            await service.Received(1).GetAsync(null);
        }

        [Fact]
        public async Task GetAsyncValidTest()
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            var filterModel = new FilterModel
            {
                Filter = "(name eq 'name')",
                Skip = 10,
                Limit = 5
            };
            var expectedViewModelList = new List<UserViewModel>
            {
                UserTestHelper.GetUserViewModel(UserTestHelper.GetUser(1)),
                UserTestHelper.GetUserViewModel(UserTestHelper.GetUser(2, "login2", "password2", "name2", Policy.ModeratorsPolicy))
            };
            service.GetAsync(filterModel).Returns(expectedViewModelList);

            // act
            var actionResult = await controller.GetAsync(filterModel);

            // act and assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualViewModelList = (List<UserViewModel>)okResult.Value;
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                UserTestHelper.AssertUserViewModels(expectedViewModelList[i], actualViewModelList[i]);
            await service.Received(1).GetAsync(filterModel);
        }

        #endregion // Get

        #region GetBy

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetByAsyncValidTest(int id)
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            var user = UserTestHelper.GetUser(id);
            var expectedUserViewModel = UserTestHelper.GetUserViewModel(user);
            service.GetByAsync(id).Returns(Task.FromResult(expectedUserViewModel));

            // act
            var actionResult = await controller.GetByAsync(id);

            // assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualViewModel = (UserViewModel)okResult.Value;
            UserTestHelper.AssertUserViewModels(expectedUserViewModel, actualViewModel);
            await service.Received(1).GetByAsync(id);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetByAsyncInvalidTest(int id)
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            var user = UserTestHelper.GetUser(id);
            var expectedUserViewModel = UserTestHelper.GetUserViewModel(user);
            service.GetByAsync(Arg.Is<int>(x => x != id)).Returns(Task.FromResult(expectedUserViewModel));
            service.GetByAsync(Arg.Is<int>(x => x == id)).Throws(new ServiceNotFoundException(typeof(User)));

            // act and assert
            await Assert.ThrowsAsync<ServiceNotFoundException>(async () => await controller.GetByAsync(id));
            await service.Received(1).GetByAsync(id);
        }

        #endregion // GetBy

        #region Save

        [Fact]
        public async Task SaveAsyncInvalidModelTest()
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            var userEditModel = UserTestHelper.GetUserEditModelByParameters(1);
            service.SaveAsync(Arg.Is<UserEditModel>(x => x.Id != null)).Throws(new ServiceValidationException());
            service.SaveAsync(Arg.Is<UserEditModel>(x => x.Id == null)).Returns(userEditModel);

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.SaveAsync(userEditModel));
            await service.Received(1).SaveAsync(userEditModel);
        }

        [Fact]
        public async Task SaveAsyncValidModelTest()
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            var userEditModel = UserTestHelper.GetUserEditModelByParameters(null);
            var expectedEditModel = UserTestHelper.GetUserEditModelByParameters(1);
            service.SaveAsync(Arg.Is<UserEditModel>(x => x.Id != null)).Throws(new ServiceValidationException());
            service.SaveAsync(Arg.Is<UserEditModel>(x => x.Id == null)).Returns(expectedEditModel);

            // act
            var actionResult = await controller.SaveAsync(userEditModel);

            // assert
            Assert.NotNull(actionResult);
            Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var okResult = actionResult.Result as CreatedAtActionResult;
            Assert.Equal(StatusCodes.Status201Created, okResult.StatusCode);
            var actualEditModel = (UserEditModel)okResult.Value;
            UserTestHelper.AssertUserEditModels(expectedEditModel, actualEditModel);
            await service.Received(1).SaveAsync(userEditModel);
        }

        #endregion // Save

        #region Update

        [Fact]
        public async Task UpdateAsyncInvalidModelTest()
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            var userEditModel = UserTestHelper.GetUserEditModelByParameters(null);
            var expectedEditModel = UserTestHelper.GetUserEditModelByParameters(1, "login2", "password2", "name2");
            service.UpdateAsync(0, Arg.Is<UserEditModel>(x => x.Id == null)).Throws(new ServiceValidationException());
            service.UpdateAsync(1, Arg.Is<UserEditModel>(x => x.Id != null)).Returns(expectedEditModel);

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.UpdateAsync(0, userEditModel));
            await service.Received(1).UpdateAsync(0, userEditModel);
        }

        [Fact]
        public async Task UpdateAsyncValidModelTest()
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            var userEditModel = UserTestHelper.GetUserEditModelByParameters(1);
            var expectedEditModel = UserTestHelper.GetUserEditModelByParameters(1, "login2", "password2", "name2");
            service.UpdateAsync(0, Arg.Is<UserEditModel>(x => x.Id == null)).Throws(new ServiceValidationException());
            service.UpdateAsync(1, Arg.Is<UserEditModel>(x => x.Id != null)).Returns(expectedEditModel);

            // act
            var actionResult = await controller.UpdateAsync(1, userEditModel);

            // assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualEditModel = (UserEditModel)okResult.Value;
            UserTestHelper.AssertUserEditModels(expectedEditModel, actualEditModel);
            await service.Received(1).UpdateAsync(1, userEditModel);
        }

        #endregion // Update

        #region Delete

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DeleteAsyncValidTest(int id)
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            var user = UserTestHelper.GetUser(id);
            var expectedUserViewModel = UserTestHelper.GetUserViewModel(user);
            service.DeleteAsync(id).Returns(Task.FromResult(expectedUserViewModel));

            // act
            var actionResult = await controller.DeleteAsync(id);

            // assert
            Assert.NotNull(actionResult); Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualViewModel = (UserViewModel)okResult.Value;
            UserTestHelper.AssertUserViewModels(expectedUserViewModel, actualViewModel);
            await service.Received(1).DeleteAsync(id);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DeleteAsyncInvalidTest(int id)
        {
            // arrange
            var service = Substitute.For<IUserService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new UsersController(loggerFactory, service);

            var user = UserTestHelper.GetUser(id);
            var expectedUserViewModel = UserTestHelper.GetUserViewModel(user);
            service.DeleteAsync(Arg.Is<int>(x => x != id)).Returns(Task.FromResult(expectedUserViewModel));
            service.DeleteAsync(Arg.Is<int>(x => x == id)).Throws(new ServiceNotFoundException(typeof(User)));

            // act and assert
            await Assert.ThrowsAsync<ServiceNotFoundException>(async () => await controller.DeleteAsync(id));
            await service.Received(1).DeleteAsync(id);
        }

        #endregion // Delete
    }
}
