using DeliciousFood.Services.Base.Model;

namespace DeliciousFood.Services.Users.Model
{
    /// <summary>
    /// Edit model to mark a food as delicious for the user
    /// </summary>
    public class UserDeliciousFoodEditModel : IEditModel
    {
        public int FoodId { get; set; }
    }
}
