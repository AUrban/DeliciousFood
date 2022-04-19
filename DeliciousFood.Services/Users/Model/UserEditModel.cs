using DeliciousFood.Services.Base.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeliciousFood.Services.Users.Model
{
    /// <summary>
    /// Edit model to edit user properties
    /// </summary>
    public class UserEditModel : IdentifiedEntityEditModel
    {
        [Required]
        public string Login { get; set; }

        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, MinLength(1)]
        public List<UserPolicyEditModel> PolicyList { get; set; }
    }
}
