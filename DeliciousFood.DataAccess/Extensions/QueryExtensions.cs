using System.Linq;

namespace DeliciousFood.DataAccess.Extensions
{
    /// <summary>
    /// IQueryable extensions
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Applying skip and limit parameters to the query
        /// </summary>
        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int skip, int? limit)
        {
            query = query.Skip(skip);
            if (limit.HasValue)
                query = query.Take(limit.Value);
            return query;
        }
    }
}
