using DeliciousFood.Services.Base.Model;
using System.ComponentModel.DataAnnotations;

namespace DeliciousFood.Services.Accounts.Model
{
    /// <summary>
    /// View model to log in user
    /// </summary>
    public class LoginViewModel : IViewModel
    {
        public string Login { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
