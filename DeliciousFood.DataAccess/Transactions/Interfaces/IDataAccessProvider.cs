using System;
using System.Data;
using System.Threading.Tasks;

namespace DeliciousFood.DataAccess
{
    /// <summary>
    /// An interface of the provider to perform datasourse (database) operation in scope of application transaction. Serves for convenient creation of transactions
    /// It's allowed using nested transaction creation, nested ones will be ignored
    /// The transaction is rolled back if an exception was thrown
    /// </summary>
    public interface IDataAccessProvider
    {
        /// <summary>
        /// Perform "void" operation
        /// </summary>
        void Run(Action action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// Perform operation returning TResult
        /// </summary>
        TResult Get<TResult>(Func<TResult> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);


        /// <summary>
        /// Perform async "void" operation
        /// </summary>
        Task RunAsync(Func<Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// Perform async operation returning TResult
        /// </summary>
        Task<TResult> GetAsync<TResult>(Func<Task<TResult>> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
