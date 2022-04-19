using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliciousFood.DataAccess.Providers
{
    /// <summary>
    /// IQueryable provider to perform various query operations
    /// </summary>
    public interface IQueryableProvider
    {
        /// <summary>
        /// Returns filtered query according to the filter string
        /// </summary>
        IQueryable<T> MakeFilterQuery<T>(IQueryable<T> query, string filter);

        /// <summary>
        /// Returns filtered query according to the parent foreign key
        /// </summary>
        IQueryable<T> MakeParentQuery<T>(IQueryable<T> query, Type parentType, int parentId);

        /// <summary>
        /// Returns query as async list
        /// </summary>
        Task<List<T>> MaskAsyncListFromQuery<T>(IQueryable<T> query);
    }
}
