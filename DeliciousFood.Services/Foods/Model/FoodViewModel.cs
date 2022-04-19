using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Base.Model;

namespace DeliciousFood.Services.Foods.Model
{
    /// <summary>
    /// View model to view food properties
    /// </summary>
    public class FoodViewModel : IdentifiedEntityViewModel
    {
        public int UserId { get; set; }

        public string UserDescription { get; set; }

        public string Title { get; set; }

        public FoodType Type { get; set; }

        public string TypeDescription { get; set; }

        public decimal NumberOfCalories { get; set; }

        public string Country { get; set; }

        public bool IsPublic { get; set; }
    }
}
