using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.Services.Base;
using DeliciousFood.Services.Extensions;
using DeliciousFood.Services.Foods.Model;

namespace DeliciousFood.Services.Foods
{
    public class FoodMapperProfile : EntityMapperProfile
    {
        /// <summary>
        /// Mapping conventions between Food and FoodViewModel/FoodEditModel
        /// </summary>
        public FoodMapperProfile()
        {
            Map<Food, FoodEditModel>();
            Map<FoodEditModel, Food>()
                .IgnoreMembers(new string[] { "DeliciousFoods", "UserId" });
            Map<Food, FoodViewModel>()
                .IgnoreMembers(new string[] { "UserDescription", "TypeDescription" });
        }
    }
}
