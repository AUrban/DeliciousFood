using DeliciousFood.Api.Extensions;
using DeliciousFood.Api.Middlewares;
using DeliciousFood.Api.Security;
using DeliciousFood.Api.Security.Options;
using DeliciousFood.Api.Settings;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Serilog;
using System.Net;

namespace DeliciousFood.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // logging
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            // Binding database connection configuration section
            services.ConfigureAndBind<ConnectionStringsSettings>(Configuration.GetSection("ConnectionStrings"));

            // controllers
            services
                .AddControllers()
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            // IoC
            services.AddIoC();

            // IHttpContextAccessor
            services.AddHttpContextAccessor();

            // HttpClient
            services.AddHttpClient();

            // Automapping
            services.AddAutoMappings();

            // Add JWT token auth
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                     .AddJwtBearer(options =>
                     {
                         options.RequireHttpsMetadata = true;
                         options.TokenValidationParameters = JWTOptions.GetTokenValidationParameters();
                     });

            // Add authorization
            services.AddAuthorization(opts => {
                opts.AddPolicy(PolicyAliases.ModeratorsAdminsPolicy,
                    policy => policy.Requirements.Add(new PolicyRequirement(Policy.ModeratorsPolicy | Policy.AdminsPolicy)));
                opts.AddPolicy(PolicyAliases.UsersAdminsPolicy,
                    policy => policy.Requirements.Add(new PolicyRequirement(Policy.UsersPolicy | Policy.AdminsPolicy)));
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Delicious food Web API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Delicious Food Web API V1");
                });

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Using Serilog
            app.UseSerilogRequestLogging();

            // Routing
            app.UseRouting();

            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Database migrations
            app.MigrateDatabase();

            // Exceptions handling
            app.UseServiceExceptionHandling(options =>
            {
                options.Bind(typeof(ServiceNotFoundException), HttpStatusCode.NotFound);
                options.Bind(typeof(ServiceValidationException), HttpStatusCode.BadRequest);
                options.Bind(typeof(ServiceUnauthorizedException), HttpStatusCode.Unauthorized);
            });

            // Data access application transactions
            app.UseMiddleware<ServiceDataAccessMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
