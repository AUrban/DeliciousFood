using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Attributes;
using DeliciousFood.Services.Base.Model;
using System.ComponentModel.DataAnnotations;

namespace DeliciousFood.Services.Foods.Model
{
    /// <summary>
    /// Edit model to edit food properties
    /// </summary>
    public class FoodEditModel : IdentifiedEntityEditModel
    {
        [Required]
        public string Title { get; set; }

        [EnumerationRequired]
        public FoodType? Type { get; set; }

        public decimal? NumberOfCalories { get; set; }

        [Required]
        public string Country { get; set; }

        public bool IsPublic { get; set; }
    }
}
