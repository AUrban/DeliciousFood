using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Base.Model;

namespace DeliciousFood.Services.Users.Model
{
    /// <summary>
    /// View model to view user properties
    /// </summary>
    public class UserViewModel : IdentifiedEntityViewModel
    {
        public string Login { get; set; }

        public string Name { get; set; }

        public Policy PolicyMask { get; set; }

        public string PolicyDescription { get; set; }
    }
}
