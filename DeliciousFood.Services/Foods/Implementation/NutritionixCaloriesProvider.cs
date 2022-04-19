using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace DeliciousFood.Services.Foods.Implementation
{
    /// <summary>
    /// A calories API provider based on Nutritionix API
    /// </summary>
    public class NutritionixCaloriesProvider : ICaloriesProvider
    {
        /// <summary>
        /// These parameters are only here because this is a test project.
        /// For the real project these options should be obtained via Environment and this shouldn't be in the git repo
        /// </summary>
        private const string BASE_URL = @"https://trackapi.nutritionix.com/v2/natural/nutrients";
        private const string APP_ID = @"463fc332";
        private const string APP_KEY = @"3a1fe7bf1f4b951ebb3976855313f239";

        private HttpClient HttpClient { get; }
        private ILogger Logger { get; }

        public NutritionixCaloriesProvider(IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        {
            HttpClient = httpClientFactory.CreateClient();
            Logger = loggerFactory.CreateLogger<NutritionixCaloriesProvider>();
        }

        public async Task<decimal> GetNumberOfCaloriesAsync(string food)
        {
            Logger.LogInformation($"Getting number of calories for the food '{food}' from Nutritionix API");

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(BASE_URL),
                    Method = HttpMethod.Post,
                    Headers =
                {
                    { "x-app-id", APP_ID },
                    { "x-app-key", APP_KEY },
                    { HttpRequestHeader.ContentType.ToString(), "application/json" }
                },
                    Content = new StringContent(JsonConvert.SerializeObject(new { query = food }), Encoding.UTF8, "application/json")
                };

                var response = await HttpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.LogWarning($"The Nutritionix API response doesn't have the success status code for food = {food}");
                    throw new ArgumentException();
                }

                var foodList = await response.Content.ReadFromJsonAsync<FoodList>();
                return (decimal)(foodList != null ? foodList.Foods?.Sum(x => x.Nf_calories) ?? 0 : 0);
            }
            catch (HttpRequestException e)
            {
                Logger.LogError(e, $"The Nutritionix API failed with the request for food = {food}");

                return 0;
            }
        }
    }

    /// <summary>
    /// POCO class describing the format of receiving data
    /// </summary>
    public class FoodList
    {
        public NutritionixFood[] Foods { get; set; }
    }
    public class NutritionixFood
    {
        public string Food_name { get; set; }
        public float Nf_calories { get; set; }
    }
}
