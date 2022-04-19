using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace DeliciousFood.DataAccess.Providers
{
    /// <summary>
    /// IQueryable provider to perform filter query based on Dynamic Linq Core
    /// </summary>
    public class DynamicLinqQueryableProvider : IQueryableProvider
    {
        private IDictionary<string, string> MapOfSubstitutions { get; set; }

        public DynamicLinqQueryableProvider()
        {
            MapOfSubstitutions = new Dictionary<string, string>
            {
                { "'", "\"" },
                { " eq ", " = " },
                { " ne ", " != " },
                { " gt ", " > " },
                { " lt ", " < " },
                { " amp ", " & "}
            };
        }

        public IQueryable<T> MakeFilterQuery<T>(IQueryable<T> query, string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return query;

            filter = MapOfSubstitutions.ToList()
                                       .Aggregate(filter, (result, x) => result.Replace(x.Key, MapOfSubstitutions[x.Key]));
            return query.Where(filter, new object[] { });
        }

        public IQueryable<T> MakeParentQuery<T>(IQueryable<T> query, Type parentType, int parentId)
        {
            return query.Where($"{parentType.Name}Id = {parentId}");
        }

        public Task<List<T>> MaskAsyncListFromQuery<T>(IQueryable<T> query)
        {
            return query.ToListAsync();
        }
    }
}
