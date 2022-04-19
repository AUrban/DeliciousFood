using DeliciousFood.Api.Controllers.Base;
using DeliciousFood.Api.Security.Options;
using DeliciousFood.Services.Base.Model;
using DeliciousFood.Services.Foods;
using DeliciousFood.Services.Foods.Model;
using DeliciousFood.Services.Users.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliciousFood.Api.Controllers
{
    /// <summary>
    /// A contoller for food records CRUD operations
    /// </summary>
    [Authorize(Policy = PolicyAliases.UsersAdminsPolicy)]
    [Route("api/users/{userId}/foods")]
    public sealed class FoodsController : BaseServiceController<IFoodService>
    {
        public FoodsController(ILoggerFactory loggerFactory, IFoodService service)
            : base(loggerFactory, service)
        {
        }

        /// <summary>
        /// Getting all food records by filter
        /// </summary>
        [HttpGet("/api/foods")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<List<FoodViewModel>>> GetAllAsync([FromQuery] FilterModel filterModel)
        {
            Logger.LogInformation($"Getting all food records with filter = {JsonConvert.SerializeObject(filterModel)}");

            return Ok(await Service.GetAllAsync(filterModel));
        }

        /// <summary>
        /// Getting food records for the user by filter
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<List<FoodViewModel>>> GetAsync(int userId, [FromQuery] FilterModel filterModel)
        {
            Logger.LogInformation($"Getting all food records with filter = {JsonConvert.SerializeObject(filterModel)}");

            await Service.SetUserIdAsync(userId);
            return Ok(await Service.GetAsync(filterModel));
        }

        /// <summary>
        /// Getting a record by its id
        /// </summary>
        [HttpGet("{id}", Name = "RecordGetBy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FoodViewModel>> GetByAsync(int userId, int id)
        {
            Logger.LogInformation($"Getting food record with id = {id}");

            await Service.SetUserIdAsync(userId);
            return Ok(await Service.GetByAsync(id));
        }

        /// <summary>
        /// Creation a record according to the edit model
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FoodViewModel>> SaveAsync(int userId, [FromBody] FoodEditModel editModel)
        {
            Logger.LogInformation($"Creation food record {JsonConvert.SerializeObject(editModel)}");

            await Service.SetUserIdAsync(userId);
            editModel = await Service.SaveAsync(editModel);
            return CreatedAtRoute("RecordGetBy", new { userId, id = editModel.Id }, editModel);
        }

        /// <summary>
        /// Updating a record by its id according to the edit model
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FoodViewModel>> UpdateAsync(int userId, int id, [FromBody] FoodEditModel editModel)
        {
            Logger.LogInformation($"Updating food record {JsonConvert.SerializeObject(editModel)}");

            await Service.SetUserIdAsync(userId);
            return Ok(await Service.UpdateAsync(id, editModel));
        }

        /// <summary>
        /// Deleting a record by its id
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FoodViewModel>> DeleteAsync(int userId, int id)
        {
            Logger.LogInformation($"Deleting food record with id = {id}");

            await Service.SetUserIdAsync(userId);
            return Ok(await Service.DeleteAsync(id));
        }

        /// <summary>
        /// Getting all public food records by filter
        /// </summary>
        [HttpGet("/api/foods/public")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<List<FoodViewModel>>> GetPublicRecordsAsync([FromQuery] FilterModel filterModel)
        {
            Logger.LogInformation($"Getting public food records with filter = {JsonConvert.SerializeObject(filterModel)}");

            return Ok(await Service.GetPublicRecordsAsync(filterModel));
        }

        /// <summary>
        /// Mark a public food record as delicious
        /// </summary>
        [HttpPost("/api/foods/delicious")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<FoodViewModel>> MarkAsDeliciousAsync([FromBody] UserDeliciousFoodEditModel editModel)
        {
            Logger.LogInformation($"Mark the delicious food record for the user {JsonConvert.SerializeObject(editModel)}");

            return Ok(await Service.MarkAsDeliciousAsync(editModel));
        }

        /// <summary>
        /// Getting delicious food records for the user by filter
        /// </summary>
        [HttpGet("/api/foods/delicious")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<List<FoodViewModel>>> GetDeliciousFoodsAsync([FromQuery] FilterModel filterModel)
        {
            Logger.LogInformation($"Getting delicious food records for the user with filter = {JsonConvert.SerializeObject(filterModel)}");

            return Ok(await Service.GetDeliciousFoodsAsync(filterModel));
        }
    }
}
