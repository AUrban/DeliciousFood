using DeliciousFood.DataAccess;
using DeliciousFood.DataAccess.Exceptions;
using DeliciousFood.Services.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DeliciousFood.Api.Middlewares
{
    /// <summary>
    /// Performing request operations in the application transaction
    /// </summary>
    public class ServiceDataAccessMiddleware
    {
        private readonly RequestDelegate Next;

        public ServiceDataAccessMiddleware(RequestDelegate next)
        {
            Next = next;
        }

        public async Task InvokeAsync(HttpContext context, IDataAccessProvider dataAccessProvider)
        {
            await dataAccessProvider.RunAsync(async () =>
            {
                try
                {
                    await Next(context);
                }
                catch (DataAccessPermissionException dpe)
                {
                    throw new ServiceValidationException(dpe.Message);
                }
            });
        }
    }
}
