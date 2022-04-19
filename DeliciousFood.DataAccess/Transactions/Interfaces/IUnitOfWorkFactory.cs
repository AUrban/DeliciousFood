using System.Data;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// A factory to create an instance of unit of work
    /// </summary>
    public interface IUnitOfWorkFactory
    {
        /// <summary>
        /// Getting a new instance of unit of work
        /// </summary>
        IUnitOfWork CreateInstance(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
