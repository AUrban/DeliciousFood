using DeliciousFood.Api.Controllers;
using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Base.Model;
using DeliciousFood.Services.Exceptions;
using DeliciousFood.Services.Foods;
using DeliciousFood.Services.Foods.Model;
using DeliciousFood.Tests.UnitTests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DeliciousFood.Tests.UnitTests.Controllers
{
    /// <summary>
    /// Unit-tests for food controller methods
    /// </summary>
    public class FoodControllerTest
    {
        public FoodControllerTest()
        {
        }

        #region GetAll

        [Fact]
        public async Task GetAllAsyncNullableModelServiceExceptionTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            service.GetAllAsync(Arg.Is<FilterModel>(x => x == null)).Throws(new ServiceValidationException());

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.GetAllAsync(null));
            await service.Received(1).GetAllAsync(null);
        }

        [Fact]
        public async Task GetAllAsyncValidTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var filterModel = new FilterModel
            {
                Filter = "(type eq 0)",
                Skip = 2,
                Limit = 3
            };
            var expectedViewModelList = new List<FoodViewModel>
            {
                FoodTestHelper.GetFoodViewModel(FoodTestHelper.GetFood(1, 1, "1 nut", FoodType.Breakfast, 100, "Russia", true)),
                FoodTestHelper.GetFoodViewModel(FoodTestHelper.GetFood(2, 1, "1 apple", FoodType.Dinner, 200, "England", false)),
                FoodTestHelper.GetFoodViewModel(FoodTestHelper.GetFood(3, 1, "1 soup", FoodType.Lunch, 350, "USA", true))
            };
            service.GetAllAsync(filterModel).Returns(expectedViewModelList);

            // act
            var actionResult = await controller.GetAllAsync(filterModel);

            // act and assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualViewModelList = (List<FoodViewModel>)okResult.Value;
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                FoodTestHelper.AssertFoodViewModels(expectedViewModelList[i], actualViewModelList[i]);
            await service.Received(1).GetAllAsync(filterModel);
        }

        #endregion // GetAll

        #region Get
      
        [Fact]
        public async Task GetAsyncInvalidUserServiceExceptionTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var filterModel = new FilterModel();
            service.SetUserIdAsync(Arg.Is<int>(x => x != 3)).Throws(new ServiceValidationException());

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.GetAsync(2, filterModel));
            await service.Received(1).SetUserIdAsync(2);
            await service.Received(0).GetAsync(filterModel);
        }

        [Fact]
        public async Task GetAsyncValidTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var filterModel = new FilterModel
            {
                Filter = "(type eq 0)",
                Skip = 2
            };
            var expectedViewModelList = new List<FoodViewModel>
            {
                FoodTestHelper.GetFoodViewModel(FoodTestHelper.GetFood(1, 1, "1 nut", FoodType.Breakfast, 100, "Russia", true)),
                FoodTestHelper.GetFoodViewModel(FoodTestHelper.GetFood(2, 1, "1 apple", FoodType.Dinner, 200, "England", false)),
            };
            service.SetUserIdAsync(Arg.Is<int>(x => x != 3)).Throws(new ServiceValidationException());
            service.GetAsync(filterModel).Returns(expectedViewModelList);

            // act
            var actionResult = await controller.GetAsync(3, filterModel);

            // act and assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualViewModelList = (List<FoodViewModel>)okResult.Value;
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                FoodTestHelper.AssertFoodViewModels(expectedViewModelList[i], actualViewModelList[i]);
            await service.Received(1).SetUserIdAsync(3);
            await service.Received(1).GetAsync(filterModel);
        }

        #endregion // Get

        #region GetBy

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetByAsyncInvalidTest(int id)
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var userId = 2;
            var food = FoodTestHelper.GetRandomFood(id, userId);
            var expectedFoodViewModel = FoodTestHelper.GetFoodViewModel(food);
            service.SetUserIdAsync(Arg.Is<int>(x => x != userId)).Throws(new ServiceValidationException());
            service.GetByAsync(Arg.Is<int>(x => x != id)).Returns(Task.FromResult(expectedFoodViewModel));
            service.GetByAsync(Arg.Is<int>(x => x == id)).Throws(new ServiceNotFoundException(typeof(Food)));

            // act and assert
            await Assert.ThrowsAsync<ServiceNotFoundException>(async () => await controller.GetByAsync(userId, id));
            await service.Received(1).GetByAsync(id);
            await service.Received(1).SetUserIdAsync(userId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetByAsyncValidTest(int id)
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var userId = 2;
            var food = FoodTestHelper.GetRandomFood(id, userId);
            var expectedFoodViewModel = FoodTestHelper.GetFoodViewModel(food);
            service.SetUserIdAsync(Arg.Is<int>(x => x != userId)).Throws(new ServiceValidationException());
            service.GetByAsync(id).Returns(Task.FromResult(expectedFoodViewModel));

            // act
            var actionResult = await controller.GetByAsync(userId,  id);

            // assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualViewModel = (FoodViewModel)okResult.Value;
            FoodTestHelper.AssertFoodViewModels(expectedFoodViewModel, actualViewModel);
            await service.Received(1).GetByAsync(id);
            await service.Received(1).SetUserIdAsync(userId);
        }

        #endregion // GetBy

        #region Save

        [Fact]
        public async Task SaveAsyncInvalidModelTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var userId = 2;
            var editModel = FoodTestHelper.GetFoodEditModel(FoodTestHelper.GetRandomFood(1, userId));
            service.SetUserIdAsync(Arg.Is<int>(x => x != userId)).Throws(new ServiceValidationException());
            service.SaveAsync(Arg.Is<FoodEditModel>(x => x.Id != null)).Throws(new ServiceValidationException());
            service.SaveAsync(Arg.Is<FoodEditModel>(x => x.Id == null)).Returns(editModel);

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.SaveAsync(userId, editModel));
            await service.Received(1).SaveAsync(editModel);
            await service.Received(1).SetUserIdAsync(userId);
        }

        [Fact]
        public async Task SaveAsyncValidTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var userId = 2;
            var editModel = FoodTestHelper.GetFoodEditModel(FoodTestHelper.GetRandomFood(1, userId));
            editModel.Id = null;
            var expectedEditModel = FoodTestHelper.GetFoodEditModel(FoodTestHelper.GetRandomFood(1, userId));
            service.SetUserIdAsync(Arg.Is<int>(x => x != userId)).Throws(new ServiceValidationException());
            service.SaveAsync(Arg.Is<FoodEditModel>(x => x.Id != null)).Throws(new ServiceValidationException());
            service.SaveAsync(Arg.Is<FoodEditModel>(x => x.Id == null)).Returns(expectedEditModel);

            // act
            var actionResult = await controller.SaveAsync(userId, editModel);

            // assert
            Assert.NotNull(actionResult);
            Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
            var okResult = actionResult.Result as CreatedAtRouteResult;
            Assert.Equal(StatusCodes.Status201Created, okResult.StatusCode);
            var actualEditModel = (FoodEditModel)okResult.Value;
            FoodTestHelper.AssertFoodEditModels(expectedEditModel, actualEditModel);
            await service.Received(1).SaveAsync(editModel);
            await service.Received(1).SetUserIdAsync(userId);
        }

        #endregion // Save

        #region Update

        [Fact]
        public async Task UpdateAsyncInvalidModelTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var userId = 2;
            var editModel = FoodTestHelper.GetFoodEditModel(FoodTestHelper.GetRandomFood(1, userId));
            editModel.Id = null;
            service.SetUserIdAsync(Arg.Is<int>(x => x != userId)).Throws(new ServiceValidationException());
            service.UpdateAsync(0, Arg.Is<FoodEditModel>(x => x.Id == null)).Throws(new ServiceValidationException());
            service.UpdateAsync(1, Arg.Is<FoodEditModel>(x => x.Id != null)).Returns(editModel);

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.UpdateAsync(userId, 0, editModel));
            await service.Received(1).UpdateAsync(0, editModel);
            await service.Received(1).SetUserIdAsync(userId);
        }

        [Fact]
        public async Task UpdateAsyncValidTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var userId = 2;
            var editModel = FoodTestHelper.GetFoodEditModel(FoodTestHelper.GetRandomFood(1, userId));
            var expectedEditModel = FoodTestHelper.GetFoodEditModel(
                FoodTestHelper.GetFood(1, userId, "1 potato", FoodType.Breakfast, 57, "Russia", true));
            service.SetUserIdAsync(Arg.Is<int>(x => x != userId)).Throws(new ServiceValidationException());
            service.UpdateAsync(0, Arg.Is<FoodEditModel>(x => x.Id == null)).Throws(new ServiceValidationException());
            service.UpdateAsync(1, Arg.Is<FoodEditModel>(x => x.Id != null)).Returns(expectedEditModel);

            // act
            var actionResult = await controller.UpdateAsync(userId, 1, editModel);

            // assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualEditModel = (FoodEditModel)okResult.Value;
            FoodTestHelper.AssertFoodEditModels(expectedEditModel, actualEditModel);
            await service.Received(1).UpdateAsync(1, editModel);
            await service.Received(1).SetUserIdAsync(userId);
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
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var userId = 2;
            var food = FoodTestHelper.GetRandomFood(id, userId);
            var expectedViewModel = FoodTestHelper.GetFoodViewModel(food);
            service.SetUserIdAsync(Arg.Is<int>(x => x != userId)).Throws(new ServiceValidationException());
            service.DeleteAsync(id).Returns(expectedViewModel);

            // act
            var actionResult = await controller.DeleteAsync(userId, id);

            // assert
            Assert.NotNull(actionResult); 
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualViewModel = (FoodViewModel)okResult.Value;
            FoodTestHelper.AssertFoodViewModels(expectedViewModel, actualViewModel);
            await service.Received(1).DeleteAsync(id);
            await service.Received(1).SetUserIdAsync(userId);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task DeleteAsyncInvalidTest(int id)
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var userId = 2;
            var food = FoodTestHelper.GetRandomFood(id, userId);
            var expectedViewModel = FoodTestHelper.GetFoodViewModel(food);
            service.SetUserIdAsync(Arg.Is<int>(x => x != userId)).Throws(new ServiceValidationException());
            service.DeleteAsync(Arg.Is<int>(x => x != id)).Returns(Task.FromResult(expectedViewModel));
            service.DeleteAsync(Arg.Is<int>(x => x == id)).Throws(new ServiceNotFoundException(typeof(Food)));

            // act and assert
            await Assert.ThrowsAsync<ServiceNotFoundException>(async () => await controller.DeleteAsync(userId, id));
            await service.Received(1).DeleteAsync(id);
            await service.Received(1).SetUserIdAsync(userId);
        }

        #endregion // Delete

        #region GetPublicRecords

        [Fact]
        public async Task GetPublicRecordsAsyncNullableModelServiceExceptionTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            service.GetPublicRecordsAsync(Arg.Is<FilterModel>(x => x == null)).Throws(new ServiceValidationException());

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.GetPublicRecordsAsync(null));
            await service.Received(1).GetPublicRecordsAsync(null);
        }

        [Fact]
        public async Task GetPublicRecordsAsyncValidTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var filterModel = new FilterModel
            {
                Filter = "(type eq 0)",
                Skip = 2,
                Limit = 3
            };
            var expectedViewModelList = new List<FoodViewModel>
            {
                FoodTestHelper.GetFoodViewModel(FoodTestHelper.GetFood(1, 1, "1 nut", FoodType.Breakfast, 100, "Russia", true)),
                FoodTestHelper.GetFoodViewModel(FoodTestHelper.GetFood(3, 2, "1 soup", FoodType.Lunch, 350, "USA", true))
            };
            service.GetPublicRecordsAsync(filterModel).Returns(expectedViewModelList);

            // act
            var actionResult = await controller.GetPublicRecordsAsync(filterModel);

            // act and assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualViewModelList = (List<FoodViewModel>)okResult.Value;
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                FoodTestHelper.AssertFoodViewModels(expectedViewModelList[i], actualViewModelList[i]);
            await service.Received(1).GetPublicRecordsAsync(filterModel);
        }

        #endregion // GetPublicRecords

        #region GetDeliciousFood

        [Fact]
        public async Task GetDeliciousFoodAsyncNullableModelServiceExceptionTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            service.GetDeliciousFoodsAsync(Arg.Is<FilterModel>(x => x == null)).Throws(new ServiceValidationException());

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await controller.GetDeliciousFoodsAsync(null));
            await service.Received(1).GetDeliciousFoodsAsync(null);
        }

        [Fact]
        public async Task GetDeliciousFoodAsyncValidTest()
        {
            // arrange
            var service = Substitute.For<IFoodService>();
            var loggerFactory = Substitute.For<ILoggerFactory>();
            var controller = new FoodsController(loggerFactory, service);

            var filterModel = new FilterModel
            {
                Filter = "(type eq 2)",
                Skip = 2,
                Limit = 3
            };
            var expectedViewModelList = new List<FoodViewModel>
            {
                FoodTestHelper.GetFoodViewModel(FoodTestHelper.GetFood(1, 1, "1 nut", FoodType.Breakfast, 100, "Russia", true)),
                FoodTestHelper.GetFoodViewModel(FoodTestHelper.GetFood(3, 1, "1 soup", FoodType.Lunch, 350, "USA", true))
            };
            service.GetDeliciousFoodsAsync(filterModel).Returns(expectedViewModelList);

            // act
            var actionResult = await controller.GetDeliciousFoodsAsync(filterModel);

            // act and assert
            Assert.NotNull(actionResult);
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            var actualViewModelList = (List<FoodViewModel>)okResult.Value;
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                FoodTestHelper.AssertFoodViewModels(expectedViewModelList[i], actualViewModelList[i]);
            await service.Received(1).GetDeliciousFoodsAsync(filterModel);
        }

        #endregion // GetDeliciousFood
    }
}
