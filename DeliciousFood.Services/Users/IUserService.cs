using DeliciousFood.Services.Base;
using DeliciousFood.Services.Users.Model;

namespace DeliciousFood.Services.Users
{
    /// <summary>
    /// User service interface
    /// </summary>
    public interface IUserService : IEntityService<UserViewModel, UserEditModel>
    {
    }
}
