using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Attributes;

namespace DeliciousFood.Services.Users.Model
{
    /// <summary>
    /// Nested edit model to edit user permissions
    /// </summary>
    public class UserPolicyEditModel
    {
        [EnumerationRequired]
        public Policy? Policy { get; set; }
    }
}
