using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Tests.UnitTests.Helpers;
using DeliciousFood.Services.Foods.Model;
using DeliciousFood.Services.Accounts.Model;
using DeliciousFood.Services.Users.Model;

namespace DeliciousFood.Tests.IntegrationTests
{
    public class FoodControllerIntegrationTest : BaseControllerIntegrationTest
    {
        public FoodControllerIntegrationTest()
        {
        }

        [Fact]
        public async Task AdminWorkingUsersTest()
        {
            // not auth
            var response = await Client.GetAsync("/api/foods");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);

            // login admin
            var loginViewModel = MapCredentials[Policy.AdminsPolicy];
            var content = new StringContent(JsonConvert.SerializeObject(loginViewModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/login", content);
            response.EnsureSuccessStatusCode();
            var tokenViewModel = await response.Content.ReadFromJsonAsync<TokenViewModel>();
            Assert.NotNull(tokenViewModel);
            Assert.NotNull(tokenViewModel.AccessToken);
            Assert.NotNull(tokenViewModel.RefreshToken);
            AddBearerToken(tokenViewModel.AccessToken);

            // get all foods
            response = await Client.GetAsync("/api/foods");
            response.EnsureSuccessStatusCode();
            var foodViewModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(foodViewModelList);

            // add own food with most default parameters
            var foodEditModel = new FoodEditModel
            {
                Title = "1 green apple",
                Type = FoodType.Breakfast,
                NumberOfCalories = 100,
                Country = "USA"
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/users/1/foods", content);
            response.EnsureSuccessStatusCode();
            var foodEditUpdatedModel = await response.Content.ReadFromJsonAsync<FoodEditModel>();
            Assert.NotNull(foodEditUpdatedModel);
            FoodTestHelper.AssertFoodEditModels(foodEditModel, foodEditUpdatedModel, true);

            // add another food for another user
            foodEditModel = new FoodEditModel
            {
                Title = "1 pear and 2 nuts",
                Type = FoodType.Dinner,
                NumberOfCalories = 200,
                Country = "England",
                IsPublic = true
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/users/3/foods", content);
            response.EnsureSuccessStatusCode();
            foodEditUpdatedModel = await response.Content.ReadFromJsonAsync<FoodEditModel>();
            Assert.NotNull(foodEditUpdatedModel);
            FoodTestHelper.AssertFoodEditModels(foodEditModel, foodEditUpdatedModel, true);

            // get food
            response = await Client.GetAsync($"/api/users/3/foods/{foodEditUpdatedModel.Id}");
            response.EnsureSuccessStatusCode();
            var foodViewModel = await response.Content.ReadFromJsonAsync<FoodViewModel>();
            Assert.NotNull(foodViewModel);
            FoodTestHelper.AssertFoodViewEditModels(foodEditUpdatedModel, foodViewModel);

            // get food
            response = await Client.GetAsync($"/api/users/1/foods/{foodEditUpdatedModel.Id}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            // edit food
            foodEditModel = new FoodEditModel
            {
                Id = foodViewModel.Id,
                Title = "1 bottle of water",
                Type = FoodType.Lunch,
                NumberOfCalories = 300,
                Country = "Italy",
                IsPublic = true
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PutAsync($"/api/users/3/foods/{foodEditModel.Id}", content);
            response.EnsureSuccessStatusCode();
            foodEditUpdatedModel = await response.Content.ReadFromJsonAsync<FoodEditModel>();
            Assert.NotNull(foodEditUpdatedModel);
            FoodTestHelper.AssertFoodEditModels(foodEditModel, foodEditUpdatedModel, true);

            // get all
            response = await Client.GetAsync($"/api/foods?skip={foodViewModelList.Count}");
            response.EnsureSuccessStatusCode();
            var refreshFoodViewModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(refreshFoodViewModelList);
            Assert.Equal(2, refreshFoodViewModelList.Count);
            FoodTestHelper.AssertFoodViewEditModels(foodEditModel, refreshFoodViewModelList[1]);

            // get for the admin
            response = await Client.GetAsync($"/api/users/1/foods?skip={foodViewModelList.Count(x => x.UserId == 1)}");
            response.EnsureSuccessStatusCode();
            var adminUserModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(adminUserModelList);
            Assert.Single(adminUserModelList);

            // get for the user
            response = await Client.GetAsync($"/api/users/3/foods?skip={foodViewModelList.Count(x => x.UserId == 3)}");
            response.EnsureSuccessStatusCode();
            var userUserModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(userUserModelList);
            Assert.Single(userUserModelList);
            FoodTestHelper.AssertFoodViewEditModels(foodEditModel, userUserModelList[0]);

            // get public records
            response = await Client.GetAsync($"/api/foods/public?skip={foodViewModelList.Count(x => x.IsPublic)}");
            response.EnsureSuccessStatusCode();
            var publicFoodViewModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(publicFoodViewModelList);
            Assert.Single(publicFoodViewModelList);
            FoodTestHelper.AssertFoodViewEditModels(foodEditModel, publicFoodViewModelList[0]);

            // mark as delicious
            var deliciousEditModel = new UserDeliciousFoodEditModel
            {
                FoodId = publicFoodViewModelList[0].Id
            };
            content = new StringContent(JsonConvert.SerializeObject(deliciousEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/foods/delicious", content);
            response.EnsureSuccessStatusCode();
            var deliciousModel = await response.Content.ReadFromJsonAsync<FoodViewModel>();
            Assert.NotNull(deliciousModel);
            FoodTestHelper.AssertFoodViewEditModels(foodEditModel, deliciousModel);

            // get delicious
            response = await Client.GetAsync($"/api/foods/delicious");
            response.EnsureSuccessStatusCode();
            var deliciousViewModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(deliciousViewModelList);
            Assert.Single(deliciousViewModelList);
            FoodTestHelper.AssertFoodViewEditModels(foodEditModel, deliciousViewModelList[0]);

            // remove user
            response = await Client.DeleteAsync($"/api/users/{adminUserModelList[0].UserId}/foods/{adminUserModelList[0].Id}");
            response.EnsureSuccessStatusCode();
            foodViewModel = await response.Content.ReadFromJsonAsync<FoodViewModel>();
            Assert.NotNull(foodViewModel);
            Assert.Equal(adminUserModelList[0].Id, foodViewModel.Id);


            ClearBearerToken();
        }

        [Fact]
        public async Task ModeratorWorkingUsersTest()
        {
            // not auth
            var response = await Client.GetAsync("/api/foods");
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);

            // login
            var loginViewModel = MapCredentials[Policy.ModeratorsPolicy];
            var content = new StringContent(JsonConvert.SerializeObject(loginViewModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/login", content);
            response.EnsureSuccessStatusCode();
            var tokenViewModel = await response.Content.ReadFromJsonAsync<TokenViewModel>();
            Assert.NotNull(tokenViewModel);
            Assert.NotNull(tokenViewModel.AccessToken);
            Assert.NotNull(tokenViewModel.RefreshToken);
            AddBearerToken(tokenViewModel.AccessToken);

            // get all
            response = await Client.GetAsync("/api/foods");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // get
            response = await Client.GetAsync("/api/users/2/foods");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // get by
            response = await Client.GetAsync($"/api/users/2/foods/1");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // add user
            var foodEditModel = new FoodEditModel
            {
                Title = "1 pear and 2 nuts",
                Type = FoodType.Breakfast,
                Country = "USA"
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/users/2/foods", content);
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // update
            foodEditModel = new FoodEditModel
            {
                Title = "1 bottle of water",
                Type = FoodType.Breakfast,
                Country = "USA"
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PutAsync("/api/users/2/foods/1", content);
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // get public records
            response = await Client.GetAsync($"/api/foods/public");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // mark as delicious
            var deliciousEditModel = new UserDeliciousFoodEditModel
            {
                FoodId = 1
            };
            content = new StringContent(JsonConvert.SerializeObject(deliciousEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/foods/delicious", content);
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // get delicious
            response = await Client.GetAsync($"/api/foods/delicious");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);

            // remove
            response = await Client.DeleteAsync($"/api/users/2/foods/1");
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UserWorkingUsersTest()
        {
            // login admin
            var loginViewModel = MapCredentials[Policy.AdminsPolicy];
            var content = new StringContent(JsonConvert.SerializeObject(loginViewModel), Encoding.UTF8, "application/json");
            var response = await Client.PostAsync("/api/login", content);
            response.EnsureSuccessStatusCode();
            var tokenViewModel = await response.Content.ReadFromJsonAsync<TokenViewModel>();
            Assert.NotNull(tokenViewModel);
            Assert.NotNull(tokenViewModel.AccessToken);
            Assert.NotNull(tokenViewModel.RefreshToken);
            AddBearerToken(tokenViewModel.AccessToken);

            response = await Client.GetAsync($"/api/foods/public");
            response.EnsureSuccessStatusCode();
            var publicFoodViewModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(publicFoodViewModelList);

            // add admin food with most default parameters
            var foodEditModel = new FoodEditModel
            {
                Title = "1 green apple",
                Type = FoodType.Snack,
                NumberOfCalories = 100,
                Country = "Russia"
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/users/1/foods", content);
            response.EnsureSuccessStatusCode();
            var foodEditUpdatedModel = await response.Content.ReadFromJsonAsync<FoodEditModel>();
            Assert.NotNull(foodEditUpdatedModel);
            int idFoodAdmin1 = foodEditUpdatedModel.Id.Value;

            // add another admin food
            foodEditModel = new FoodEditModel
            {
                Title = "1 pear and 2 nuts",
                NumberOfCalories = 200,
                Type = FoodType.Snack,
                Country = "Spain",
                IsPublic = true
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/users/1/foods", content);
            response.EnsureSuccessStatusCode();
            foodEditUpdatedModel = await response.Content.ReadFromJsonAsync<FoodEditModel>();
            Assert.NotNull(foodEditUpdatedModel);
            int idFoodAdmin2 = foodEditUpdatedModel.Id.Value;

            ClearBearerToken();


            // login user
            loginViewModel = MapCredentials[Policy.UsersPolicy];
            content = new StringContent(JsonConvert.SerializeObject(loginViewModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/login", content);
            response.EnsureSuccessStatusCode();
            tokenViewModel = await response.Content.ReadFromJsonAsync<TokenViewModel>();
            Assert.NotNull(tokenViewModel);
            Assert.NotNull(tokenViewModel.AccessToken);
            Assert.NotNull(tokenViewModel.RefreshToken);
            AddBearerToken(tokenViewModel.AccessToken);

            // add user food
            foodEditModel = new FoodEditModel
            {
                Title = "1 yellow pear",
                Type = FoodType.Breakfast,
                NumberOfCalories = 100,
                Country = "USA",
                IsPublic = true
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/users/3/foods", content);
            response.EnsureSuccessStatusCode();
            foodEditUpdatedModel = await response.Content.ReadFromJsonAsync<FoodEditModel>();
            var deleteableModelId = foodEditUpdatedModel.Id;
            Assert.NotNull(foodEditUpdatedModel);
            FoodTestHelper.AssertFoodEditModels(foodEditModel, foodEditUpdatedModel, true);

            // add another user food
            foodEditModel = new FoodEditModel
            {
                Title = "3 pear and 5 nuts",
                Type = FoodType.Breakfast,
                NumberOfCalories = 300,
                Country = "Russia",
                IsPublic = true
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/users/3/foods", content);
            response.EnsureSuccessStatusCode();
            foodEditUpdatedModel = await response.Content.ReadFromJsonAsync<FoodEditModel>();
            Assert.NotNull(foodEditUpdatedModel);
            FoodTestHelper.AssertFoodEditModels(foodEditModel, foodEditUpdatedModel, true);

            // get user food
            response = await Client.GetAsync($"/api/users/3/foods/{foodEditUpdatedModel.Id}");
            response.EnsureSuccessStatusCode();
            var foodViewModel = await response.Content.ReadFromJsonAsync<FoodViewModel>();
            Assert.NotNull(foodViewModel);
            FoodTestHelper.AssertFoodViewEditModels(foodEditUpdatedModel, foodViewModel);

            // get admin food
            response = await Client.GetAsync($"/api/users/1/foods/{idFoodAdmin1}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            // get foods - only own foods
            response = await Client.GetAsync("/api/foods");
            response.EnsureSuccessStatusCode();
            var foodViewModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(foodViewModelList);
            foreach (var viewModel in foodViewModelList)
                Assert.Equal(viewModel.UserId, 3);

            // edit food
            foodEditModel = new FoodEditModel
            {
                Id = foodViewModel.Id,
                Title = "5 nuts",
                Type = FoodType.Lunch,
                NumberOfCalories = 120,
                Country = "Russia",
                IsPublic = true
            };
            content = new StringContent(JsonConvert.SerializeObject(foodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PutAsync($"/api/users/3/foods/{foodViewModel.Id}", content);
            response.EnsureSuccessStatusCode();
            foodEditUpdatedModel = await response.Content.ReadFromJsonAsync<FoodEditModel>();
            Assert.NotNull(foodEditUpdatedModel);
            FoodTestHelper.AssertFoodEditModels(foodEditModel, foodEditUpdatedModel, true);

            // edit admin food
            var adminFoodEditModel = new FoodEditModel
            {
                Id = idFoodAdmin2,
                Title = "banana",
                Type = FoodType.Snack,
                NumberOfCalories = 90,
                Country = "Turkey"
            };
            content = new StringContent(JsonConvert.SerializeObject(adminFoodEditModel), Encoding.UTF8, "application/json");
            response = await Client.PutAsync($"/api/users/1/foods/{idFoodAdmin2}", content);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

            // get public records
            response = await Client.GetAsync($"/api/foods/public?skip={publicFoodViewModelList.Count}");
            response.EnsureSuccessStatusCode();
            publicFoodViewModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(publicFoodViewModelList);
            Assert.Equal(3, publicFoodViewModelList.Count);
            FoodTestHelper.AssertFoodViewEditModels(foodEditModel, publicFoodViewModelList[2]);

            // mark as delicious
            var deliciousEditModel = new UserDeliciousFoodEditModel
            {
                FoodId = publicFoodViewModelList[2].Id
            };
            content = new StringContent(JsonConvert.SerializeObject(deliciousEditModel), Encoding.UTF8, "application/json");
            response = await Client.PostAsync("/api/foods/delicious", content);
            response.EnsureSuccessStatusCode();
            var deliciousModel = await response.Content.ReadFromJsonAsync<FoodViewModel>();
            Assert.NotNull(deliciousModel);
            FoodTestHelper.AssertFoodViewEditModels(foodEditModel, deliciousModel);

            // get delicious
            response = await Client.GetAsync($"/api/foods/delicious");
            response.EnsureSuccessStatusCode();
            var deliciousViewModelList = await response.Content.ReadFromJsonAsync<List<FoodViewModel>>();
            Assert.NotNull(deliciousViewModelList);
            Assert.Single(deliciousViewModelList);
            FoodTestHelper.AssertFoodViewEditModels(foodEditModel, deliciousViewModelList[0]);

            // remove food
            response = await Client.DeleteAsync($"/api/users/3/foods/{deleteableModelId}");
            response.EnsureSuccessStatusCode();
            foodViewModel = await response.Content.ReadFromJsonAsync<FoodViewModel>();
            Assert.NotNull(foodViewModel);
            Assert.Equal(deleteableModelId, foodViewModel.Id);

            // remove admin food
            response = await Client.DeleteAsync($"/api/users/1/foods/{idFoodAdmin1}");
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);


            ClearBearerToken();
        }
    }
}
