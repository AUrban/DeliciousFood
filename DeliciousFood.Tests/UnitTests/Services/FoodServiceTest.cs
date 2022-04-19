using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.DataAccess.Repositories;
using DeliciousFood.Services.Base.Model;
using DeliciousFood.Services.Exceptions;
using DeliciousFood.Services.Foods;
using DeliciousFood.Services.Foods.Implementation;
using DeliciousFood.Services.Foods.Model;
using DeliciousFood.Services.Security;
using DeliciousFood.Services.Users.Model;
using DeliciousFood.Tests.UnitTests.Helpers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DeliciousFood.Tests.UnitTests.Services
{
    public class FoodServiceTest : BaseServiceTest
    {
        public FoodServiceTest()
        {
        }

        #region SetUserId

        [Fact]
        public async Task SetUserIdAsyncInvalidTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator, 
                                           repository, userRepository, deliciousRepository);

            var userId = 1;
            userSessionProvider.UserId.Returns(2);
            policyValidator.ValidateUserPolicyIntersect(Policy.AdminsPolicy).Returns(false);

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await service.SetUserIdAsync(userId));
            policyValidator.Received(1).ValidateUserPolicyIntersect(Policy.AdminsPolicy);
            await userRepository.Received(0).GetAsync(userId);
        }

        [Fact]
        public async Task SetUserIdAsyncValidTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var userId = 1;
            userSessionProvider.UserId.Returns(2);
            policyValidator.ValidateUserPolicyIntersect(Policy.AdminsPolicy).Returns(true);
            var user = UserTestHelper.GetUser(userId);
            userRepository.GetAsync(userId).Returns(user);

            // act
            await service.SetUserIdAsync(userId);

            // assert
            policyValidator.Received(1).ValidateUserPolicyIntersect(Policy.AdminsPolicy);
            await userRepository.Received(1).GetAsync(userId);
            UserTestHelper.AssertUsers(user, service.UserScope);
        }

        #endregion // SetUderId

        #region GetAll

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetAllAsyncTest(bool isAdmin)
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var fullFoodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 4),
                FoodTestHelper.GetRandomFood(4, 1), FoodTestHelper.GetRandomFood(5, 3)
            };
            var cutFoodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 4)
            };
            var fullQuery = fullFoodList.AsQueryable();
            repository.UntrackedQuery.Returns(fullQuery);
            var cutQuery = cutFoodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(cutQuery);
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());
            policyValidator.ValidateUserPolicyIntersect(Policy.AdminsPolicy).Returns(isAdmin);
            var filterModel = new FilterModel
            {
                Skip = 1
            };

            // act
            var actualViewModelList = await service.GetAllAsync(filterModel);

            // assert
            Assert.NotNull(actualViewModelList);
            var expectedViewModelList = isAdmin
                ? new List<FoodViewModel> {
                    FoodTestHelper.GetFoodViewModel(fullFoodList[1]),
                    FoodTestHelper.GetFoodViewModel(fullFoodList[2]),
                    FoodTestHelper.GetFoodViewModel(fullFoodList[3]),
                    FoodTestHelper.GetFoodViewModel(fullFoodList[4])}
                : new List<FoodViewModel> {
                    FoodTestHelper.GetFoodViewModel(cutFoodList[1]),
                    FoodTestHelper.GetFoodViewModel(cutFoodList[2])};
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                FoodTestHelper.AssertFoodViewModels(expectedViewModelList[i], actualViewModelList[i]);
            queryableProvider.Received(1).MakeFilterQuery(isAdmin ? fullQuery : cutQuery, null);
            await queryableProvider.ReceivedWithAnyArgs(1).MaskAsyncListFromQuery(isAdmin ? fullQuery : cutQuery);
            policyValidator.Received(1).ValidateUserPolicyIntersect(Policy.AdminsPolicy);
            userRepository.Received(isAdmin ? 0 : 1).GetSubRepository<Food>(Arg.Any<User>());
        }

        #endregion // GetAll

        #region Get

        [Fact]
        public async Task GetAsyncValidTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 4),
                FoodTestHelper.GetRandomFood(4, 1), FoodTestHelper.GetRandomFood(5, 3)
            };
            var query = foodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(query);
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());
            var filterModel = new FilterModel
            {
                Skip = 1
            };

            // act
            var actualViewModelList = await service.GetAsync(filterModel);

            // assert
            Assert.NotNull(actualViewModelList);
            var expectedViewModelList = new List<FoodViewModel> {
                    FoodTestHelper.GetFoodViewModel(foodList[1]),
                    FoodTestHelper.GetFoodViewModel(foodList[2]),
                    FoodTestHelper.GetFoodViewModel(foodList[3]),
                    FoodTestHelper.GetFoodViewModel(foodList[4])};
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                FoodTestHelper.AssertFoodViewModels(expectedViewModelList[i], actualViewModelList[i]);
            queryableProvider.Received(1).MakeFilterQuery(query, null);
            await queryableProvider.ReceivedWithAnyArgs(1).MaskAsyncListFromQuery(query);
            userRepository.Received(1).GetSubRepository<Food>(Arg.Any<User>());
        }

        #endregion // Get

        #region GetBy

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async Task GetByAsyncValidTest(int id)
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);
            
            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 4),
                FoodTestHelper.GetRandomFood(4, 1), FoodTestHelper.GetRandomFood(5, 3)
            };
            var query = foodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(query);
            var food = foodList.FirstOrDefault(x => x.Id == id);
            subRepository.GetAsync(id).Returns(food);
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());            

            // act
            var actualViewModel = await service.GetByAsync(id);

            // assert
            Assert.NotNull(actualViewModel);
            var expectedViewModel = FoodTestHelper.GetFoodViewModel(food);
            FoodTestHelper.AssertFoodViewModels(expectedViewModel, actualViewModel);
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await subRepository.Received(1).GetAsync(id);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(23)]
        [InlineData(45)]
        public async Task GetByAsyncInvalidTest(int id)
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 4),
                FoodTestHelper.GetRandomFood(4, 1), FoodTestHelper.GetRandomFood(5, 3)
            };
            var query = foodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(query);
            var food = foodList.FirstOrDefault(x => x.Id == id);
            subRepository.GetAsync(id).Returns(food);
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());

            // act and assert
            await Assert.ThrowsAsync<ServiceNotFoundException>(async () => await service.GetByAsync(id));
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await subRepository.Received(1).GetAsync(id);
        }

        #endregion // GetBy

        #region Save

        [Fact]
        public async Task SaveAsyncValidTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 2),
                FoodTestHelper.GetRandomFood(4, 2), FoodTestHelper.GetRandomFood(5, 3), FoodTestHelper.GetRandomFood(6, 3)
            };
            var food = FoodTestHelper.GetRandomFood(0, 2);
            var editModel = FoodTestHelper.GetFoodEditModel(food);
            editModel.Id = null;

            var query = foodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(query);
            subRepository.Create().Returns(Activator.CreateInstance(typeof(Food)));
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());
            caloriesProvider.GetNumberOfCaloriesAsync(Arg.Any<string>()).Returns(120);

            // act
            var actualEditModel = await service.SaveAsync(editModel);

            // assert
            Assert.NotNull(actualEditModel);
            food.NumberOfCalories = 120;
            var expectedEditModel = FoodTestHelper.GetFoodEditModel(food);
            FoodTestHelper.AssertFoodEditModels(expectedEditModel, actualEditModel);
            await subRepository.ReceivedWithAnyArgs(1).SaveAsync(default);
        }

        [Fact]
        public async Task SaveAsyncInvalidModelTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 2),
                FoodTestHelper.GetRandomFood(4, 2), FoodTestHelper.GetRandomFood(5, 3), FoodTestHelper.GetRandomFood(6, 3)
            };
            var food = FoodTestHelper.GetRandomFood(3, 2);
            var editModel = FoodTestHelper.GetFoodEditModel(food);

            var query = foodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(query);
            subRepository.Create().Returns(Activator.CreateInstance(typeof(Food)));
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());
            caloriesProvider.GetNumberOfCaloriesAsync(Arg.Any<string>()).Returns(120);

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await service.SaveAsync(editModel));
            await subRepository.ReceivedWithAnyArgs(0).SaveAsync(default);
        }

        #endregion // Save

        #region Update

        [Theory]
        [InlineData(3)]
        [InlineData(null)]
        public async Task UpdateAsyncValidTest(int? foodId)
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 2),
                FoodTestHelper.GetRandomFood(4, 2), FoodTestHelper.GetRandomFood(5, 3), FoodTestHelper.GetRandomFood(6, 3)
            };
            var food = FoodTestHelper.GetRandomFood(3, 2);
            var editModel = FoodTestHelper.GetFoodEditModel(food);
            editModel.Id = foodId;

            var query = foodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(query);
            subRepository.Create().Returns(Activator.CreateInstance(typeof(Food)));
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());
            caloriesProvider.GetNumberOfCaloriesAsync(Arg.Any<string>()).Returns(120);
            subRepository.GetAsync(3).Returns(food);

            // act
            var actualEditModel = await service.UpdateAsync(3, editModel);

            // assert
            Assert.NotNull(actualEditModel);
            food.NumberOfCalories = 120;
            var expectedEditModel = FoodTestHelper.GetFoodEditModel(food);
            FoodTestHelper.AssertFoodEditModels(expectedEditModel, actualEditModel);
            await subRepository.ReceivedWithAnyArgs(1).UpdateAsync(default);
        }

        [Fact]
        public async Task UpdateAsyncInvalidModelTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 2),
                FoodTestHelper.GetRandomFood(4, 2), FoodTestHelper.GetRandomFood(5, 3), FoodTestHelper.GetRandomFood(6, 3)
            };
            var food = FoodTestHelper.GetRandomFood(3, 2);
            var editModel = FoodTestHelper.GetFoodEditModel(food);
            editModel.Id = 4;

            var query = foodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(query);
            subRepository.Create().Returns(Activator.CreateInstance(typeof(Food)));
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());
            caloriesProvider.GetNumberOfCaloriesAsync(Arg.Any<string>()).Returns(120);


            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await service.UpdateAsync(3, editModel));
            await subRepository.ReceivedWithAnyArgs(0).UpdateAsync(default);
        }

        #endregion // Update

        #region Delete

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public async Task DeleteAsyncValidTest(int id)
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);
            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 4),
                FoodTestHelper.GetRandomFood(4, 1), FoodTestHelper.GetRandomFood(5, 3)
            };
            var query = foodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(query);
            var food = foodList.FirstOrDefault(x => x.Id == id);
            subRepository.GetAsync(id).Returns(food);
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());

            // act
            var actualViewModel = await service.DeleteAsync(id);

            // assert
            Assert.NotNull(actualViewModel);
            var expectedViewModel = FoodTestHelper.GetFoodViewModel(food);
            FoodTestHelper.AssertFoodViewModels(expectedViewModel, actualViewModel);
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await subRepository.Received(1).DeleteAsync(food);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(23)]
        [InlineData(45)]
        public async Task DeleteAsyncInvalidTest(int id)
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1), FoodTestHelper.GetRandomFood(3, 4),
                FoodTestHelper.GetRandomFood(4, 1), FoodTestHelper.GetRandomFood(5, 3)
            };
            var query = foodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<Food, User>>();
            subRepository.UntrackedQuery.Returns(query);
            var food = foodList.FirstOrDefault(x => x.Id == id);
            subRepository.GetAsync(id).Returns(food);
            userRepository.GetSubRepository<Food>(Arg.Any<User>()).Returns(subRepository);
            userRepository.Query.Returns(users.AsQueryable());

            // act and assert
            await Assert.ThrowsAsync<ServiceNotFoundException>(async () => await service.DeleteAsync(id));
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await repository.Received(0).DeleteAsync(food);
        }

        #endregion // Delete

        #region GetPublicRecords

        [Fact]
        public async Task GetPublicRecordsAsyncTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1, true), FoodTestHelper.GetRandomFood(2, 1), 
                FoodTestHelper.GetRandomFood(3, 4, true),
                FoodTestHelper.GetRandomFood(4, 1), FoodTestHelper.GetRandomFood(5, 3, true)
            };
            var query = foodList.AsQueryable();
            repository.UntrackedQuery.Returns(query);
            userRepository.Query.Returns(users.AsQueryable());
            var filterModel = new FilterModel()
            {
                Limit = 2
            };

            // act
            var actualViewModelList = await service.GetPublicRecordsAsync(filterModel);

            // assert
            Assert.NotNull(actualViewModelList);
            var expectedViewModelList = new List<FoodViewModel> {
                    FoodTestHelper.GetFoodViewModel(foodList[0]),
                    FoodTestHelper.GetFoodViewModel(foodList[2])};
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                FoodTestHelper.AssertFoodViewModels(expectedViewModelList[i], actualViewModelList[i]);
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await queryableProvider.ReceivedWithAnyArgs(1).MaskAsyncListFromQuery(query);
        }

        #endregion // GetPublicRecords

        #region MarkAsDelicious

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        public async Task MarkAsDeliciousAsyncTest(int foodId)
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1), FoodTestHelper.GetRandomFood(2, 1, true, "Russia"), 
                FoodTestHelper.GetRandomFood(3, 1, true, "England"),
                FoodTestHelper.GetRandomFood(4, 1, true, "Russia"), FoodTestHelper.GetRandomFood(5, 3), 
                FoodTestHelper.GetRandomFood(6, 3)
            };
            var userDeliciousFoodList = new List<UserDeliciousFood>
            {
                new UserDeliciousFood { UserId = 1, FoodId = 1, Food = foodList[0] },
                new UserDeliciousFood { UserId = 1, FoodId = 2, Food = foodList[1] }
            };
            var userId = 1;
            var user = users.Find(x => x.Id == userId);
            var food = foodList.Find(x => x.Id == foodId);
            var editModel = new UserDeliciousFoodEditModel { FoodId = foodId };
            userSessionProvider.UserId.Returns(1);
            userRepository.Query.Returns(users.AsQueryable());
            var query = userDeliciousFoodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<UserDeliciousFood, User>>();
            subRepository.Create().Returns(Activator.CreateInstance(typeof(UserDeliciousFood)));
            subRepository.Query.Returns(query);
            userRepository.GetSubRepository<UserDeliciousFood>(user).Returns(subRepository);
            userRepository.GetAsync(1).Returns(user);
            repository.GetAsync(foodId).Returns(food);

            // act
            var actualEditModel = await service.MarkAsDeliciousAsync(editModel);

            // assert
            Assert.NotNull(actualEditModel);
            var expectedEditModel = FoodTestHelper.GetFoodViewModel(food);
            FoodTestHelper.AssertFoodViewModels(expectedEditModel, actualEditModel);
            await subRepository.ReceivedWithAnyArgs(1).SaveAsync(default);
            await repository.Received(1).GetAsync(foodId);
            subRepository.Received(1).Create();
            await subRepository.Received(foodId == 4 ? 1 : 0).DeleteAsync(userDeliciousFoodList[1]);
        }

        [Fact]
        public async Task MarkAsDeliciousAsyncInvalidTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1, false), FoodTestHelper.GetRandomFood(2, 1, true, "Russia"),
                FoodTestHelper.GetRandomFood(3, 1, true, "England"),
                FoodTestHelper.GetRandomFood(4, 1, true, "Russia"), FoodTestHelper.GetRandomFood(5, 3),
                FoodTestHelper.GetRandomFood(6, 3)
            };
            var userDeliciousFoodList = new List<UserDeliciousFood>
            {
                new UserDeliciousFood { UserId = 1, FoodId = 1, Food = foodList[0] },
                new UserDeliciousFood { UserId = 1, FoodId = 2, Food = foodList[1] }
            };
            var userId = 1;
            var user = users.Find(x => x.Id == userId);
            var foodId = 1;
            var food = foodList.Find(x => x.Id == foodId);
            var editModel = new UserDeliciousFoodEditModel { FoodId = foodId };
            userSessionProvider.UserId.Returns(1);
            userRepository.Query.Returns(users.AsQueryable());
            var query = userDeliciousFoodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<UserDeliciousFood, User>>();
            subRepository.Create().Returns(Activator.CreateInstance(typeof(UserDeliciousFood)));
            subRepository.Query.Returns(query);
            userRepository.GetSubRepository<UserDeliciousFood>(user).Returns(subRepository);
            userRepository.GetAsync(1).Returns(user);
            repository.GetAsync(foodId).Returns(food);

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await service.MarkAsDeliciousAsync(editModel));
            await repository.Received(1).GetAsync(foodId);
            await subRepository.ReceivedWithAnyArgs(0).SaveAsync(default);            
            subRepository.Received(0).Create();
        }

        #endregion // MarkAsDelicious

        #region GetDeliciousFoods

        [Fact]
        public async Task GetDeliciousFoodsAsyncTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<Food>();
            var repository = Substitute.For<IEntityRepository<Food>>();
            var caloriesProvider = Substitute.For<ICaloriesProvider>();
            var userSessionProvider = Substitute.For<IUserSessionProvider>();
            var policyValidator = GetPolicyValidator();
            var userRepository = Substitute.For<IEntityRepository<User>>();
            var deliciousRepository = Substitute.For<IEntityRepository<UserDeliciousFood>>();
            var service = new FoodService(mapper, queryableProvider,
                                           caloriesProvider, userSessionProvider, policyValidator,
                                           repository, userRepository, deliciousRepository);

            var users = new List<User> { UserTestHelper.GetUser(1), UserTestHelper.GetUser(4), UserTestHelper.GetUser(3) };
            var foodList = new List<Food>
            {
                FoodTestHelper.GetRandomFood(1, 1, true), FoodTestHelper.GetRandomFood(2, 1),
                FoodTestHelper.GetRandomFood(3, 4, true),
                FoodTestHelper.GetRandomFood(4, 1), FoodTestHelper.GetRandomFood(5, 3, true)
            };
            var userDeliciousFoodList = new List<UserDeliciousFood>
            {
                new UserDeliciousFood { UserId = 1, FoodId = 1, Food = foodList[0] },
                new UserDeliciousFood { UserId = 1, FoodId = 2, Food = foodList[1] }
            };
            userSessionProvider.UserId.Returns(1);
            userRepository.Query.Returns(users.AsQueryable());
            var query = userDeliciousFoodList.AsQueryable();
            var subRepository = Substitute.For<ISubEntityRepository<UserDeliciousFood, User>>();
            subRepository.UntrackedQuery.Returns(query);
            userRepository.GetSubRepository<UserDeliciousFood>(Arg.Any<User>()).Returns(subRepository);
            userRepository.GetAsync(1).Returns(users[0]);
            var filterModel = new FilterModel();

            // act
            var actualViewModelList = await service.GetDeliciousFoodsAsync(filterModel);

            // assert
            Assert.NotNull(actualViewModelList);
            var expectedViewModelList = new List<FoodViewModel> {
                    FoodTestHelper.GetFoodViewModel(foodList[0]),
                    FoodTestHelper.GetFoodViewModel(foodList[1])};
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                FoodTestHelper.AssertFoodViewModels(expectedViewModelList[i], actualViewModelList[i]);
            await userRepository.Received(1).GetAsync(1);
        }

        #endregion // GetDeliciousFoods
    }
}
