using DeliciousFood.Api.Controllers.Base;
using DeliciousFood.Services.Accounts;
using DeliciousFood.Services.Accounts.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DeliciousFood.Api.Controllers
{
    /// <summary>
    /// Controller for user authentication
    /// </summary>
    [Route("api/[action]")]
    public sealed class AccountsController : BaseServiceController<IAccountService>
    {
        public AccountsController(ILoggerFactory loggerFactory, IAccountService service)
            : base(loggerFactory, service)
        {
        }

        /// <summary>
        /// User login to the system.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(TokenViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<TokenViewModel>> LoginAsync(LoginViewModel model)
        {
            Logger.LogInformation($"Login user: {JsonConvert.SerializeObject(model)}");

            return Ok(await Service.LoginAsync(model));
        }

        /// <summary>
        /// User logout
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LogoutAsync([BindRequired] string refreshToken)
        {
            Logger.LogInformation($"Logout user with token {refreshToken}");

            await Service.LogoutAsync(refreshToken);
            return Ok();
        }

        /// <summary>
        /// Called from the client when the access token has expired
        /// A new access-token is returned, and there is no need to log in again
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        public async Task<ActionResult<string>> RefreshAsync([BindRequired] string refreshToken)
        {
            Logger.LogInformation($"Refresh access token by {refreshToken}");

            return Ok(await Service.RefreshAsync(refreshToken));
        }
    }
}
