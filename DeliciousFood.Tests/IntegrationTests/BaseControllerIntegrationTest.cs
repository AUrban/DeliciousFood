using DeliciousFood.Api;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Accounts.Model;
using DeliciousFood.Services.Users.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace DeliciousFood.Tests.IntegrationTests
{
    public class BaseControllerIntegrationTest
    {
        protected TestServer Server { get; }
        protected HttpClient Client { get; }

        protected List<UserEditModel> ListAdditionalUsers { get; } = new List<UserEditModel>
        {
            new UserEditModel
            { 
                Login = "moderator",
                Password = "Moderator@1",
                Name = "Moderator",
                PolicyList = new List<UserPolicyEditModel> { new UserPolicyEditModel { Policy = Policy.ModeratorsPolicy } }
            },
            new UserEditModel
            { 
                Login = "user",
                Password = "User@1",
                Name = "User",
                PolicyList = new List<UserPolicyEditModel> { new UserPolicyEditModel { Policy = Policy.UsersPolicy } }
            }
        };

        protected Dictionary<Policy, LoginViewModel> MapCredentials { get; } =
            new Dictionary<Policy, LoginViewModel>
            {
                { Policy.AdminsPolicy, new LoginViewModel { Login = "admin", Password = "Admin@1"} },
                { Policy.ModeratorsPolicy, new LoginViewModel { Login = "moderator", Password = "Moderator@1"} },
                { Policy.UsersPolicy, new LoginViewModel { Login = "user", Password = "User@1"} }
            };

        public BaseControllerIntegrationTest()
        {
            Server = new TestServer(new WebHostBuilder()
                .UseSerilog()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("testsettings.json");
                })
                .UseStartup<Startup>());
            Client = Server.CreateClient();

            AddAdditionalsUsers();
        }


        protected void AddAdditionalsUsers()
        {
            var loginViewModel = MapCredentials[Policy.AdminsPolicy];
            var content = new StringContent(JsonConvert.SerializeObject(loginViewModel), Encoding.UTF8, "application/json");
            var response = Client.PostAsync("/api/login", content).Result;
            response.EnsureSuccessStatusCode();
            var tokenViewModel = response.Content.ReadFromJsonAsync<TokenViewModel>().Result;
            AddBearerToken(tokenViewModel.AccessToken);

            foreach (var editModel in ListAdditionalUsers)
            {
                content = new StringContent(JsonConvert.SerializeObject(editModel), Encoding.UTF8, "application/json");
                response = Client.PostAsync("/api/users", content).Result;
                var code = response.StatusCode;
                bool val = response.IsSuccessStatusCode;
            }

            ClearBearerToken();
        }

        protected void AddBearerToken(string token)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected void ClearBearerToken()
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", null);
        }
    }
}
