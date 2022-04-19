using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// Typed repository interface for CRUD operations
    /// </summary>
    public interface IRepository<T> : IDisposable where T : class
    {
        /// <summary>
        /// Query for mutable records
        /// </summary>
        IQueryable<T> Query { get; }

        /// <summary>
        /// Query for immutable records
        /// </summary>
        IQueryable<T> UntrackedQuery { get; }


        /// <summary>
        /// Create a new record.
        /// </summary>
        T Create();


        /// <summary>
        /// Getting record by identifier
        /// </summary>
        T Get(int id);

        /// <summary>
        /// Getting record by identifier
        /// </summary>
        Task<T> GetAsync(int id);


        /// <summary>
        /// Saving a new record
        /// </summary>
        void Save(T entity);

        /// <summary>
        ///  Saving a new record
        /// </summary>
        Task SaveAsync(T entity);


        /// <summary>
        /// Updating an existing record
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Updating an existing record
        /// </summary>
        Task UpdateAsync(T entity);


        /// <summary>
        /// Deleting a record.
        /// </summary>
        void Delete(T entity);

        /// <summary>
        /// Deleting a record.
        /// </summary>
        Task DeleteAsync(T entity);
    }
}
