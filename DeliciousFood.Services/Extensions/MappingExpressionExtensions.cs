using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace DeliciousFood.Services.Extensions
{
    /// <summary>
    /// Extension methods for automapper
    /// </summary>
    public static class MappingExpressionExtensions
    {
        /// <summary>
        /// Ignore given members in the array over mapping
        /// </summary>
        public static IMappingExpression<TSource, TDest> IgnoreMembers<TSource, TDest>(this IMappingExpression<TSource, TDest> expression, string[] members)
        {
            return expression.IgnoreMembers(members.AsEnumerable());
        }

        /// <summary>
        /// Ignore given members in the enumeration over mapping
        /// </summary>
        public static IMappingExpression<TSource, TDest> IgnoreMembers<TSource, TDest>(this IMappingExpression<TSource, TDest> expression, IEnumerable<string> members)
        {
            foreach (var memberName in members)
                expression.ForMember(memberName, o => o.Ignore());
            return expression;
        }
    }
}
