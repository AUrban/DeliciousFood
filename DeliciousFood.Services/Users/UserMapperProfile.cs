using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.Services.Base;
using DeliciousFood.Services.Extensions;
using DeliciousFood.Services.Users.Model;

namespace DeliciousFood.Services.Users
{
    public class UserMapperProfile : EntityMapperProfile
    {
        /// <summary>
        /// Mapping conventions between User and UserViewModel/UserEditModel
        /// </summary>
        public UserMapperProfile()
        {
            Map<User, UserEditModel>()
                .IgnoreMembers(new string[] { "Password", "PolicyList" });
            Map<UserEditModel, User>()
                .IgnoreMembers(new string[] { "PasswordHash", "PolicyMask", "DeliciousFoods" });
            Map<User, UserViewModel>()
                .IgnoreMembers(new string[] { "PolicyDescription" });
        }
    }
}
