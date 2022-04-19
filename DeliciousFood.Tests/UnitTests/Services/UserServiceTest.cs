using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.DataAccess.Repositories;
using DeliciousFood.Services.Base.Model;
using DeliciousFood.Services.Exceptions;
using DeliciousFood.Services.Security;
using DeliciousFood.Services.Users.Implementation;
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
    public class UserServiceTest : BaseServiceTest
    {
        public UserServiceTest()
        {
        }

        #region Get

        [Fact]
        public async Task GetAsyncValidTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);

            var filterModel = new FilterModel
            {
                Skip = 1,
                Limit = 2
            };

            // act
            var actualViewModelList = await service.GetAsync(filterModel);

            // assert
            Assert.NotNull(actualViewModelList);
            var expectedViewModelList = new List<UserViewModel>
            {
                UserTestHelper.GetUserViewModel(userList[1]),
                UserTestHelper.GetUserViewModel(userList[2]),
            };
            Assert.Equal(expectedViewModelList.Count, actualViewModelList.Count);
            for (int i = 0; i < expectedViewModelList.Count; i++)
                UserTestHelper.AssertUserViewModels(expectedViewModelList[i], actualViewModelList[i]);
            queryableProvider.Received(1).MakeFilterQuery(query, null);
            await queryableProvider.ReceivedWithAnyArgs(1).MaskAsyncListFromQuery(query);
        }

        [Fact]
        public async Task GetAsyncServiceExceptionTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await service.GetAsync(null));
            queryableProvider.ReceivedWithAnyArgs(0).MakeFilterQuery(query, default);
            await queryableProvider.ReceivedWithAnyArgs(0).MaskAsyncListFromQuery(query);
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
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            User user = userList.FirstOrDefault(x => x.Id == id);
            repository.GetAsync(id).Returns(user);
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);

            // act
            var actualViewModel = await service.GetByAsync(id);

            // assert
            Assert.NotNull(actualViewModel);
            var expectedViewModel = UserTestHelper.GetUserViewModel(user);
            UserTestHelper.AssertUserViewModels(expectedViewModel, actualViewModel);
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await repository.Received(1).GetAsync(id);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(23)]
        [InlineData(45)]
        public async Task GetByAsyncInvalidTest(int id)
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            User user = userList.FirstOrDefault(x => x.Id == id);
            repository.GetAsync(id).Returns(user);
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);

            // act and assert
            await Assert.ThrowsAsync<ServiceNotFoundException>(async () => await service.GetByAsync(id));
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await repository.Received(1).GetAsync(id);
        }

        #endregion // GetBy

        #region Save

        [Fact]
        public async Task SaveAsyncValidTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            var user = UserTestHelper.GetUser(0);
            var editModel = UserTestHelper.GetUserEditModel(user);
            editModel.Id = null;
            var userUpdated = UserTestHelper.GetUser(0);
            repository.When(x => x.SaveAsync(user)).Do(x => userUpdated.Id = 6);
            repository.Create().Returns(Activator.CreateInstance(typeof(User)));
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);
            securityProvider.CheckPasswordComplexity(editModel.Password).Returns(string.Empty);
            securityProvider.GetHashedPassword(Arg.Any<string>()).Returns(args => args.ArgAt<string>(0));

            // act
            var actualEditModel = await service.SaveAsync(editModel);

            // assert
            Assert.NotNull(actualEditModel);
            var expectedEditModel = UserTestHelper.GetUserEditModel(userUpdated);
            UserTestHelper.AssertUserEditModels(expectedEditModel, actualEditModel);
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await repository.ReceivedWithAnyArgs(1).SaveAsync(default);
            securityProvider.ReceivedWithAnyArgs(1).GetHashedPassword(default);
            securityProvider.Received(1).CheckPasswordComplexity(editModel.Password);
        }

        [Fact]
        public async Task SaveAsyncInValidModelTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            var user = UserTestHelper.GetUser(1);
            var editModel = UserTestHelper.GetUserEditModel(user);
            editModel.Id = null;
            var userUpdated = UserTestHelper.GetUser(1);
            repository.When(x => x.SaveAsync(user)).Do(x => userUpdated.Id = 6);
            repository.Create().Returns(Activator.CreateInstance(typeof(User)));
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);
            securityProvider.CheckPasswordComplexity(editModel.Password).Returns(string.Empty);
            securityProvider.GetHashedPassword(Arg.Any<string>()).Returns(args => args.ArgAt<string>(0));

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await service.SaveAsync(editModel));
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await repository.ReceivedWithAnyArgs(0).SaveAsync(default);
            securityProvider.ReceivedWithAnyArgs(0).GetHashedPassword(default);
            securityProvider.Received(0).CheckPasswordComplexity(editModel.Password);
        }

        #endregion // Save

        #region Update

        [Fact]
        public async Task UpdateAsyncValidTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            var user = UserTestHelper.GetUser(3);
            var editModel = UserTestHelper.GetUserEditModel(user);
            editModel.Name = "name6";
            editModel.PolicyList = new List<UserPolicyEditModel> { new UserPolicyEditModel { Policy = Policy.UsersPolicy } } ;
            repository.GetAsync(user.Id).Returns(user);
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);
            securityProvider.CheckPasswordComplexity(editModel.Password).Returns(string.Empty);
            securityProvider.GetHashedPassword(Arg.Any<string>()).Returns(args => args.ArgAt<string>(0));

            // act
            var actualEditModel = await service.UpdateAsync(3, editModel);

            // assert
            Assert.NotNull(actualEditModel);
            UserTestHelper.AssertUserEditModels(editModel, actualEditModel);
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await repository.ReceivedWithAnyArgs(1).UpdateAsync(default);
            securityProvider.ReceivedWithAnyArgs(1).GetHashedPassword(default);
            securityProvider.Received(1).CheckPasswordComplexity(editModel.Password);
        }

        [Fact]
        public async Task UpdateAsyncInvalidModelTest()
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            var user = UserTestHelper.GetUser(3);
            var editModel = UserTestHelper.GetUserEditModel(user);
            editModel.Id = null;
            editModel.Name = "name6";
            editModel.PolicyList = new List<UserPolicyEditModel> { new UserPolicyEditModel { Policy = Policy.UsersPolicy } };
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);

            // act and assert
            await Assert.ThrowsAsync<ServiceValidationException>(async () => await service.UpdateAsync(7, editModel));
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await repository.ReceivedWithAnyArgs(0).UpdateAsync(default);
            securityProvider.ReceivedWithAnyArgs(0).GetHashedPassword(default);
            securityProvider.Received(0).CheckPasswordComplexity(editModel.Password);
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
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            User user = userList.FirstOrDefault(x => x.Id == id);
            repository.GetAsync(id).Returns(user);
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);

            // act
            var actualViewModel = await service.DeleteAsync(id);

            // assert
            Assert.NotNull(actualViewModel);
            var expectedViewModel = UserTestHelper.GetUserViewModel(user);
            UserTestHelper.AssertUserViewModels(expectedViewModel, actualViewModel);
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await repository.Received(1).DeleteAsync(user);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(23)]
        [InlineData(45)]
        public async Task DeleteAsyncInvalidTest(int id)
        {
            // arrange
            var mapper = GetMapperMock();
            var queryableProvider = GetQueryableProviderMock<User>();
            var repository = Substitute.For<IEntityRepository<User>>();
            var securityProvider = Substitute.For<ISecurityProvider>();
            var policyValidator = GetPolicyValidator();
            var service = new UserService(mapper, queryableProvider, repository, securityProvider,
                policyValidator);

            var userList = new List<User>
            {
                UserTestHelper.GetUser(1), UserTestHelper.GetUser(2), UserTestHelper.GetUser(3),
                UserTestHelper.GetUser(4), UserTestHelper.GetUser(5)
            };
            User user = userList.FirstOrDefault(x => x.Id == id);
            repository.GetAsync(id).Returns(user);
            var query = userList.AsQueryable();
            repository.UntrackedQuery.Returns(query);

            // act and assert
            await Assert.ThrowsAsync<ServiceNotFoundException>(async () => await service.DeleteAsync(id));
            queryableProvider.Received(0).MakeFilterQuery(query, null);
            await repository.Received(0).DeleteAsync(user);
        }

        #endregion // Delete
    }
}
