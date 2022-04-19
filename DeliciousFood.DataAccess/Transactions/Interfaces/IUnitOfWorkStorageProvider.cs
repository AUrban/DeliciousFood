using DeliciousFood.DataAccess.Repositories;

namespace DeliciousFood.DataAccess.Transactions
{
    /// <summary>
    /// Interface for the storage to keep the current unit of work
    /// </summary>
    public interface IUnitOfWorkStorageProvider
    {
        /// <summary>
        /// Getting the current unit of work
        /// </summary>
        IUnitOfWork Current { get; }

        /// <summary>
        /// Adding a new instance of unit of work to storage.
        /// </summary>
        void Add(IUnitOfWork unitOfWork);

        /// <summary>
        /// Deleting the unit of work from storage.
        /// </summary>
        void Remove(IUnitOfWork unitOfWork);
    }
}
