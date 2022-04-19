using System;
using System.Collections.Generic;
using System.Linq;

namespace DeliciousFood.Services.Extensions
{
    /// <summary>
    /// Extensions methods for enumeration
    /// </summary>
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Getting a list of all enums of the specified type
        /// </summary>
        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : Enum
            => Enum.GetValues(typeof(TEnum)).Cast<TEnum>();

        /// <summary>
        /// Get the name of the enum
        /// </summary>
        public static string GetName<TEnum>(this TEnum value) => Enum.GetName(typeof(TEnum), value);
    }
}
