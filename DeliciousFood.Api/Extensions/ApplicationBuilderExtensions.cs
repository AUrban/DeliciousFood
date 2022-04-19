using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using DeliciousFood.Api.Middlewares;
using DeliciousFood.Services.Exceptions;

namespace DeliciousFood.Api.Extensions
{
    /// <summary>
    /// Extension methods for the application builder. Called in the Startup file
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Database migrations lauching
        /// </summary>
        public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DbContext>();
            context.Database.Migrate();
            return app;
        }

        /// <summary>
        /// Exception handling
        /// </summary>
        public static IApplicationBuilder UseServiceExceptionHandling(this IApplicationBuilder builder, Action<ServiceExceptionHandlerOptions> optionsAction)
        {
            var options = new ServiceExceptionHandlerOptions();
            optionsAction(options);
            return builder.UseMiddleware<ServiceExceptionHandlerMiddleware>(options);
        }
    }
}
