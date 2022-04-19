using AutoMapper;
using DeliciousFood.DataAccess.DataModels.Base;
using DeliciousFood.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeliciousFood.Services.Base
{
    /// <summary>
    /// A basic entity mapping profile: some default options and ignoring unnecessary fields
    /// </summary>
    public class EntityMapperProfile : Profile
    {
        /// <summary>
        /// A basic entity mapper
        /// </summary>
        public EntityMapperProfile()
        {
            AllowNullCollections = true;
            AllowNullDestinationValues = true;
        }

        /// <summary>
        /// Base mapping method from source class to destination class
        /// </summary>
        protected virtual IMappingExpression<TSource, TDestination> Map<TSource, TDestination>()
        {
            var ignoreMembers = GetEntityIgnoreMembers(typeof(TDestination));
            return CreateMap<TSource, TDestination>().IgnoreMembers(ignoreMembers);
        }

        /// <summary>
        /// Getting default entities ignore members list
        /// </summary>
        private IEnumerable<string> GetEntityIgnoreMembers(Type entityType)
        {
            foreach (PropertyInfo propertyInfo in entityType.GetProperties())
            {
                if (propertyInfo.PropertyType.IsGenericType)
                {
                    Type type = propertyInfo.PropertyType.GetGenericTypeDefinition();
                    if (type.IsAssignableFrom(typeof(IList<>)))
                        yield return propertyInfo.Name;
                }
            }

            foreach (Type propType in entityType.GetInterfaces())
            {
                if (propType.IsGenericType &&
                    propType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ISubEntity<>)))
                    yield return propType.GenericTypeArguments[0].Name;
            }
        }
    }
}
