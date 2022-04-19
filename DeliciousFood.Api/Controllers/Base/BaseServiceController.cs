using DeliciousFood.Services.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DeliciousFood.Api.Controllers.Base
{
    /// <summary>
    /// A basic controller based on a service. The service contains all the request business logic
    /// Default attributes are indicated
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseServiceController<TService> : ControllerBase where TService : IService
    {
        /// <summary>
        /// Logger object
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Service for wrapping controller logic 
        /// </summary>
        protected TService Service { get; }


        public BaseServiceController(ILoggerFactory loggerFactory, TService service)
        {
            Logger = loggerFactory.CreateLogger($"{GetType().Name}");
            Service = service;
        }
    }
}
