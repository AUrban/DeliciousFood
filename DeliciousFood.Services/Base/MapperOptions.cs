using AutoMapper;
using DeliciousFood.Services.Foods;
using DeliciousFood.Services.Users;
using System;

namespace DeliciousFood.Services.Base
{
    /// <summary>
    /// Class for configuring the automapper
    /// </summary>
    public static class MapperOptions
    {
        /// <summary>
        /// Getting automapper instance with necessary options
        /// </summary>
        public static IMapper ProvideMapper(Func<Type, object> constructor)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                if (constructor != null)
                    cfg.ConstructServicesUsing(constructor);

                cfg.RecognizePrefixes("Id");  // RecognizePostfixes
                cfg.RecognizeDestinationPrefixes("Id");  // RecognizePostfixes

                cfg.AddProfile<EntityMapperProfile>();
                cfg.AddProfile<UserMapperProfile>();
                cfg.AddProfile<FoodMapperProfile>();
            });
            mapperConfiguration.AssertConfigurationIsValid();
            return mapperConfiguration.CreateMapper();
        }
    }
}
