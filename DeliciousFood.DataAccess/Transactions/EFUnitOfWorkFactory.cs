using DeliciousFood.DataAccess.Transactions;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// A factory implementation to create an entity framework unit of work
    /// </summary>
    public class EFUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly DbContext DbContext;
        private readonly IUnitOfWorkStorageProvider UnitOfWorkStorageProvider;

        public EFUnitOfWorkFactory(DbContext dbContext, IUnitOfWorkStorageProvider unitOfWorkStorageProvider)
        {
            DbContext = dbContext;
            UnitOfWorkStorageProvider = unitOfWorkStorageProvider;
        }

        public IUnitOfWork CreateInstance(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return new EFUnitOfWork(DbContext, UnitOfWorkStorageProvider, isolationLevel);
        }
    }
}
