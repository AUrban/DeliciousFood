using DeliciousFood.DataAccess.Repositories;
using System;

namespace DeliciousFood.DataAccess.Transactions
{
    /// <summary>
    /// Unit of work storage implementation based on keeping a single instance of unit of work
    /// </summary>
    public class UnitOfWorkSingleStorageProvider : IUnitOfWorkStorageProvider
    {
        public IUnitOfWork Current { get; private set; }

        public void Add(IUnitOfWork unitOfWork)
        {
            if (Current != null)
                throw new ArgumentException("Current UnitOfWork isn't closed");

            Current = unitOfWork;
        }

        public void Remove(IUnitOfWork unitOfWork)
        {
            if (Current != unitOfWork)
                throw new ArgumentException("Attempt to remove another UnitOfWork");

            Current = null;
        }
    }
}
