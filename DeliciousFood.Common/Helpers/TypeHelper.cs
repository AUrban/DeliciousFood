using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliciousFood.Common.Helpers
{
    /// <summary>
    /// Helpers methods for Type
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Getting types which implement given generic interface
        /// </summary>
        public static IEnumerable<Type> WithImplementedGenericInterface(this IEnumerable<Type> source, Type genericType)
        {
            return source.Where(givenType => givenType.WithImplementedGenericInterface(genericType));
        }

        public static bool WithImplementedGenericInterface(this Type givenType, Type genericType, Type argumentType = null)
        {
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType &&
                (argumentType == null || givenType.GetGenericArguments().Contains(argumentType)))
                return true;

            var interfaceTypes = givenType.GetInterfaces();
            foreach (var it in interfaceTypes)
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType &&
                    (argumentType == null || it.GetGenericArguments().Contains(argumentType)))
                    return true;

            return false;
        }


        /// <summary>
        /// Getting types which are inherited from the base one
        /// </summary>
        public static IEnumerable<Type> WithBase(this IEnumerable<Type> source, Type baseType)
        {
            return source.Where(t => t.BasedOn(baseType));
        }

        /// <summary>
        /// Checking that the given type is inherited from the base one
        /// </summary>
        public static bool BasedOn(this Type source, Type baseType)
        {
            return baseType.IsAssignableFrom(source) && baseType != source;
        }

        /// <summary>
        /// Checking that the given type is inherited from the base one
        /// </summary>
        public static bool BasedOn<T>(this Type source)
        {
            return source.BasedOn(typeof(T));
        }
    }
}
