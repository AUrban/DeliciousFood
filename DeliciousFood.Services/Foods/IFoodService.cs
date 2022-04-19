using DeliciousFood.Services.Base;
using DeliciousFood.Services.Base.Model;
using DeliciousFood.Services.Foods.Model;
using DeliciousFood.Services.Users.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliciousFood.Services.Foods
{
    /// <summary>
    /// Food service interface
    /// </summary>
    public interface IFoodService : IEntityService<FoodViewModel, FoodEditModel>
    {
        /// <summary>
        /// Setting id user scope for the request
        /// </summary>
        Task SetUserIdAsync(int userId);

        /// <summary>
        /// Getting all food by filter
        /// </summary>
        Task<List<FoodViewModel>> GetAllAsync(FilterModel requestModel);

        /// <summary>
        /// Getting all public foods by filter
        /// </summary>
        Task<List<FoodViewModel>> GetPublicRecordsAsync(FilterModel requestModel);

        /// <summary>
        /// Mark a public food as delicious
        /// </summary>
        Task<FoodViewModel> MarkAsDeliciousAsync(UserDeliciousFoodEditModel editModel);

        /// <summary>
        /// Getting delicious foods for the user by filter
        /// </summary>
        Task<List<FoodViewModel>> GetDeliciousFoodsAsync(FilterModel requestModel);
    }
}
