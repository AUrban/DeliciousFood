using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Accounts.Model;
using DeliciousFood.Services.Users.Model;
using DeliciousFood.Tests.UnitTests.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DeliciousFood.Tests.IntegrationTests
{
    public class UserControllerIntegrationTest : BaseControllerIntegrationTest
    {
        public UserControllerIntegrationTest()
        {
        }

        [Fact]
        public async Task AdminWorkingUsersTest()
        {
            await AdminOrModeratorWorkingUsersTest(Policy.AdminsPolicy);
        }

        [Fact]
        public async Task ModeratorWorkingUsersTest()
        {
            await AdminOrModeratorWorkingUsersTest(Policy.ModeratorsPolicy);
        }

        [Fact]
        public async Task UserWorkingUsersTest()
        {
            // not auth - get users
            var response = await Client.GetAsync("/api/users");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);

            // login
            var loginViewModel = MapCredentials[Policy.UsersPolicy];
            var content = new StringContent(JsonConvert.SerializeObject(loginViewModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/login", content);
            response.EnsureSuccessStatusCode();
            var tokenViewModel = await response.Content.ReadFromJsonAsync<TokenViewModel>();
            Assert.NotNull(tokenViewModel);
            Assert.NotNull(tokenViewModel.AccessToken);
            Assert.NotNull(tokenViewModel.RefreshToken);
            AddBearerToken(tokenViewModel.AccessToken);

            // get users
            response = await Client.GetAsync("/api/users");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // get the user
            response = await Client.GetAsync("/api/users/1");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // add user
            var userEditModel = new UserEditModel
            {
                Login = new Random().Next().ToString(),
                Password = "jhsajh@6Ft",
                Name = "new user",
                PolicyList = new List<UserPolicyEditModel>
                {
                    new UserPolicyEditModel { Policy = Policy.UsersPolicy },
                    new UserPolicyEditModel { Policy = Policy.ModeratorsPolicy }
                }
            };
            content = new StringContent(JsonConvert.SerializeObject(userEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/users", content);
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // update
            userEditModel = new UserEditModel
            {
                Id = 1,
                Login = new Random().Next().ToString(),
                Password = "jhsajh@6Ft",
                Name = "updated user",
                PolicyList = new List<UserPolicyEditModel>
                {
                    new UserPolicyEditModel { Policy = Policy.UsersPolicy }
                }
            };
            content = new StringContent(JsonConvert.SerializeObject(userEditModel), Encoding.UTF8, "application/json");
            response = await Client.PutAsync("/api/users/1", content);
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // remove
            response = await Client.DeleteAsync("/api/users/1");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }


        private async Task AdminOrModeratorWorkingUsersTest(Policy policy)
        {
            // not auth
            var response = await Client.GetAsync("/api/users");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);

            // login
            var loginViewModel = MapCredentials[policy];
            var content = new StringContent(JsonConvert.SerializeObject(loginViewModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/login", content);
            response.EnsureSuccessStatusCode();
            var tokenViewModel = await response.Content.ReadFromJsonAsync<TokenViewModel>();
            Assert.NotNull(tokenViewModel);
            Assert.NotNull(tokenViewModel.AccessToken);
            Assert.NotNull(tokenViewModel.RefreshToken);
            AddBearerToken(tokenViewModel.AccessToken);

            // get all users
            response = await Client.GetAsync("/api/users");
            response.EnsureSuccessStatusCode();
            var userViewModelList = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            Assert.NotNull(userViewModelList);
            Assert.True(userViewModelList.Count >= 3);

            // add user
            var userEditModel = new UserEditModel
            {
                Login = new Random().Next().ToString(),
                Password = "jhsajh@6Ft",
                Name = "new user",
                PolicyList = new List<UserPolicyEditModel>
                {
                    new UserPolicyEditModel { Policy = Policy.UsersPolicy },
                    new UserPolicyEditModel { Policy = Policy.ModeratorsPolicy }
                }
            };
            content = new StringContent(JsonConvert.SerializeObject(userEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/users", content);
            response.EnsureSuccessStatusCode();
            var userEditUpdatedModel = await response.Content.ReadFromJsonAsync<UserEditModel>();
            Assert.NotNull(userEditUpdatedModel);
            UserTestHelper.AssertUserEditModels(userEditModel, userEditUpdatedModel, true);

            // get user
            response = await Client.GetAsync($"/api/users/{userEditUpdatedModel.Id}");
            response.EnsureSuccessStatusCode();
            var userViewModel = await response.Content.ReadFromJsonAsync<UserViewModel>();
            Assert.NotNull(userViewModel);
            UserTestHelper.AssertUserViewEditModels(userEditUpdatedModel, userViewModel);

            // edit user
            userEditModel = new UserEditModel
            {
                Id = userViewModel.Id,
                Login = new Random().Next().ToString(),
                Password = "jhsajh@6Ft",
                Name = "updated user",
                PolicyList = new List<UserPolicyEditModel>
                {
                    new UserPolicyEditModel { Policy = Policy.UsersPolicy }
                }
            };
            content = new StringContent(JsonConvert.SerializeObject(userEditModel), Encoding.UTF8, "application/json");
            response = await Client.PutAsync($"/api/users/{userViewModel.Id}", content);
            response.EnsureSuccessStatusCode();
            userEditUpdatedModel = await response.Content.ReadFromJsonAsync<UserEditModel>();
            Assert.NotNull(userEditUpdatedModel);
            UserTestHelper.AssertUserEditModels(userEditModel, userEditUpdatedModel, true);

            // get all
            response = await Client.GetAsync($"/api/users?skip={userViewModelList.Count}");
            response.EnsureSuccessStatusCode();
            userViewModelList = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            Assert.NotNull(userViewModelList);
            Assert.Single(userViewModelList);
            UserTestHelper.AssertUserViewEditModels(userEditModel, userViewModelList[0]);

            // remove user
            response = await Client.DeleteAsync($"/api/users/{userEditUpdatedModel.Id}");
            response.EnsureSuccessStatusCode();
            userViewModel = await response.Content.ReadFromJsonAsync<UserViewModel>();
            Assert.NotNull(userViewModel);
            Assert.Equal(userEditModel.Id, userViewModel.Id);

            ClearBearerToken();
        }
    }
}
