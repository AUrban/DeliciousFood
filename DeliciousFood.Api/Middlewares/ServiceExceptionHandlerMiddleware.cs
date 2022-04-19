using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using DeliciousFood.Services.Exceptions;

namespace DeliciousFood.Api.Middlewares
{
    /// <summary>
    /// Handling exceptions middleware 
    /// </summary>
    public class ServiceExceptionHandlerMiddleware
    {
        private readonly RequestDelegate Next;
        private readonly ServiceExceptionHandlerOptions Options;

        public ServiceExceptionHandlerMiddleware(RequestDelegate next, ServiceExceptionHandlerOptions options)
        {
            Next = next;
            Options = options;
        }

        public async Task InvokeAsync(HttpContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                await Next(context);
            }
            catch (ServiceException ex)
            {
                var logger = loggerFactory.CreateLogger<ServiceExceptionHandlerMiddleware>();

                var exType = ex.GetType();
                if (!Options.Contains(exType))
                {
                    logger.LogError(ex, "Unexpected service exception!");
                    throw ex;
                }
                
                logger.LogWarning(ex, "Service exception. Related http code should be returned!");

                var response = context.Response;
                response.StatusCode = (int)Options.GetHttpStatusCode(exType);

                if (!string.IsNullOrEmpty(ex.Error))
                {
                    response.ContentType = "application/json";
                    var errors = new Dictionary<string, IEnumerable<string>> { { ex.KeyError, new List<string> { ex.Error } } };
                    await response.WriteAsync(JsonConvert.SerializeObject(new { errors }));
                }
            }
        }
    }
}
