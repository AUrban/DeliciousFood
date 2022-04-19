using DeliciousFood.Api.Controllers.Base;
using DeliciousFood.Api.Security.Options;
using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.Services.Users;
using DeliciousFood.Services.Users.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace DeliciousFood.Api.Controllers
{
    /// <summary>
    /// A contoller for users CRUD operations
    /// </summary>
    [Authorize(Policy = PolicyAliases.ModeratorsAdminsPolicy)]
    public sealed class UsersController : EntityController<IUserService, User, UserViewModel, UserEditModel>
    {
        public UsersController(ILoggerFactory loggerFactory, IUserService service)
            : base(loggerFactory, service)
        {
        }
    }
}
