using DeliciousFood.Api.Security;
using DeliciousFood.Api.Security.Implementation;
using DeliciousFood.Api.Settings;
using DeliciousFood.DataAccess;
using DeliciousFood.DataAccess.MsSqlServer;
using DeliciousFood.DataAccess.Providers;
using DeliciousFood.DataAccess.Repositories;
using DeliciousFood.DataAccess.Transactions;
using DeliciousFood.Services.Accounts;
using DeliciousFood.Services.Accounts.Implementation;
using DeliciousFood.Services.Base;
using DeliciousFood.Services.Foods;
using DeliciousFood.Services.Foods.Implementation;
using DeliciousFood.Services.Security;
using DeliciousFood.Services.Security.Implementation;
using DeliciousFood.Services.Users;
using DeliciousFood.Services.Users.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DeliciousFood.Api.Extensions
{
    /// <summary>
    /// IServiceCollection extensions to register all the abstractions with corresponding implementations (IoC)
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configuring all the types for IoC
        /// </summary>
        public static IServiceCollection AddIoC(this IServiceCollection services)
        {
            return services.AddDbContext()
                           .AddDataAccess()
                           .AddProviders()
                           .AddServiceLayer()
                           .AddPermissions();
        }

        /// <summary>
        /// Configuring ORM database context for IoC
        /// </summary>
        public static IServiceCollection AddDbContext(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var connectionStrings = serviceProvider.GetRequiredService<ConnectionStringsSettings>();
            return services.AddDbContext<DbContext, MsSqlServerDbContext>(options =>
            {
                options
                .UseLazyLoadingProxies()
                .UseSqlServer(connectionStrings.DbConfiguration, x => x.MigrationsAssembly(connectionStrings.MigrationProject));
            });
        }

        /// <summary>
        /// Configuring all the data access implementations for IoC
        /// </summary>
        public static IServiceCollection AddDataAccess(this IServiceCollection services)
        {
            return services.AddScoped<IUnitOfWorkFactory, EFUnitOfWorkFactory>()
                           .AddScoped<IUnitOfWorkStorageProvider, UnitOfWorkSingleStorageProvider>()
                           .AddScoped<IDataAccessProvider, DataAccessProvider>()
                           .AddScoped(typeof(IEntityRepository<>), typeof(EntityRepository<>));
        }

        /// <summary>
        /// Configuring services implementations for IoC
        /// </summary>
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            return services.AddScoped<IAccountService, AccountService>()
                           .AddScoped<IUserService, UserService>()
                           .AddScoped<IFoodService, FoodService>();
        }

        /// <summary>
        /// Configuring providers implementations for IoC
        /// </summary>
        public static IServiceCollection AddProviders(this IServiceCollection services)
        {
            return services.AddSingleton<IQueryableProvider, DynamicLinqQueryableProvider>()
                           .AddSingleton<ISecurityProvider, SecurityProvider>()
                           .AddTransient<IUserSessionProvider, ClaimsUserSessionProvider>()
                           .AddTransient<IPolicyValidator, PolicyValidator>()
                           .AddTransient<IClaimsPrincipalProvider, ClaimsPrincipalProvider>()
                           .AddTransient<ITokenProvider, JWTTokenProvider>()
                           .AddTransient<ICaloriesProvider, NutritionixCaloriesProvider>();
        }

        /// <summary>
        /// Add auth policy
        /// </summary>
        public static IServiceCollection AddPermissions(this IServiceCollection services)
        {
            return services.AddTransient<IAuthorizationHandler, PolicyHandler>();
        }

        /// <summary>
        /// Configuring automapper for IoC
        /// </summary>
        public static void AddAutoMappings(this IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                return MapperOptions.ProvideMapper(type => provider.GetService(type));
            });
        }

        /// <summary>
        /// Getting configuration
        /// </summary>
        public static TOptions ConfigureAndBind<TOptions>(this IServiceCollection services, IConfigurationSection section)
            where TOptions : class, new()
        {
            services.Configure<TOptions>(section);
            services.AddScoped(sp => sp.GetRequiredService<IOptionsSnapshot<TOptions>>().Value);
            return section.Get<TOptions>();
        }
    }
}
