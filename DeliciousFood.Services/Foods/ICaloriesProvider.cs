using System.Threading.Tasks;

namespace DeliciousFood.Services.Foods
{
    /// <summary>
    /// Interface for some calories API provider to get various information about food
    /// </summary>
    public interface ICaloriesProvider
    {
        /// <summary>
        /// Getting a number of calories for given food
        /// </summary>
        public Task<decimal> GetNumberOfCaloriesAsync(string food);
    }
}
