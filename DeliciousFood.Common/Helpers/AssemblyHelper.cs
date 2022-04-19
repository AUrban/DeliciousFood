using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeliciousFood.Common.Helpers
{
    /// <summary>
    /// Helpers methods for Assembly
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        /// Get the assembly containing the given type
        /// </summary>
        public static Assembly GetAssemblyOf(Type targetType)
        {
            return targetType.Assembly;
        }

        /// <summary>
        /// Get the current assembly
        /// </summary>
        public static Assembly GetCurrentAssembly()
        {
            return Assembly.GetCallingAssembly();
        }


        /// <summary>
        /// Getting all the types from the assembly containing the given type
        /// </summary>
        public static IEnumerable<Type> FromAssemblyOf<T>()
        {
            return FromAssemblyOf(typeof(T));
        }

        /// <summary>
        /// Getting all the types from the assembly containing the given type
        /// </summary>
        public static IEnumerable<Type> FromAssemblyOf(Type targetType)
        {
            return GetAssemblyOf(targetType).GetTypes().AsEnumerable();
        }

        /// <summary>
        /// Getting all the types from the current assembly
        /// </summary>
        public static IEnumerable<Type> FromCurrentAssembly()
        {
            return GetCurrentAssembly().GetTypes().AsEnumerable();
        }
    }
}
