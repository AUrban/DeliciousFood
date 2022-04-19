using DeliciousFood.DataAccess.DataModels.Base;
using DeliciousFood.Services.Base;
using DeliciousFood.Services.Base.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliciousFood.Api.Controllers.Base
{
    /// <summary>
    /// Controller for entities CRUD operations
    /// </summary>
    public abstract class EntityController<TService, TModel, TViewModel, TEditModel> : BaseServiceController<TService>
        where TService : IEntityService<TViewModel, TEditModel>
        where TModel : class, IEntity, new()
        where TViewModel : class, IIdentifiedViewModel, new()
        where TEditModel : class, IIdentifiedEditModel, new()
    {
        public EntityController(ILoggerFactory loggerFactory, TService service)
            : base(loggerFactory, service)
        {
        }

        /// <summary>
        /// Getting records by filter
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<List<TViewModel>>> GetAsync([FromQuery] FilterModel filterModel)
        {
            Logger.LogInformation($"Getting {typeof(TModel).Name} records with filter = {JsonConvert.SerializeObject(filterModel)}");

            return Ok(await Service.GetAsync(filterModel));
        }

        /// <summary>
        /// Getting a record by its id
        /// </summary>
        [HttpGet("{id}", Name = "GetBy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<TViewModel>> GetByAsync(int id)
        {
            Logger.LogInformation($"Getting {typeof(TModel).Name} record with id = {id}");

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
        public async Task<ActionResult<TEditModel>> SaveAsync([FromBody] TEditModel editModel)
        {
            Logger.LogInformation($"Creation {typeof(TModel).Name} record {JsonConvert.SerializeObject(editModel)}");

            editModel = await Service.SaveAsync(editModel);
            return CreatedAtAction("GetBy", new { editModel.Id }, editModel);
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
        public async Task<ActionResult<TEditModel>> UpdateAsync(int id, [FromBody] TEditModel editModel)
        {
            Logger.LogInformation($"Updating {typeof(TModel).Name} record {JsonConvert.SerializeObject(editModel)}");

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
        public async Task<ActionResult<TViewModel>> DeleteAsync(int id)
        {
            Logger.LogInformation($"Deleting {typeof(TModel).Name} record with id = {id}");

            return Ok(await Service.DeleteAsync(id));
        }
    }
}
