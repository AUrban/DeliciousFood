using System;
using System.Threading.Tasks;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// UnitOfWork pattern interface with supporting async operations
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Commit all changes made in the context in the database
        /// </summary>
        void Commit();

        /// <summary>
        /// Async commit all changes made in the context in the database
        /// </summary>
        Task CommitAsync();


        /// <summary>
        /// Rollback all changes made in the context in the database
        /// </summary>
        void Rollback();

        /// <summary>
        /// Asdync rollback all changes made in the context in the database
        /// </summary>
        Task RollbackAsync();


        /// <summary>
        /// Completed commit flag
        /// </summary>
        bool Commited { get; }
    }
}
