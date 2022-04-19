using DeliciousFood.DataAccess.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// UnitOfWork pattern implementation for Entity Framework Core based on DbContext
    /// </summary>
    public class EFUnitOfWork : IEFUnitOfWork
    {
        private bool disposed = false;


        public DbContext DbContext { get; }
        public bool Commited { get; private set; }


        private IUnitOfWorkStorageProvider UnitOfWorkStorageProvider { get; set; }
        private IsolationLevel IsolationLevel { get; set; }
        private IDbContextTransaction Transaction { get; set; }
        

        public EFUnitOfWork(DbContext dbContext, IUnitOfWorkStorageProvider unitOfWorkStorageProvider, IsolationLevel isolationLevel)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            UnitOfWorkStorageProvider = unitOfWorkStorageProvider;
            IsolationLevel = isolationLevel;

            Transaction = DbContext.Database.BeginTransaction(IsolationLevel);
            UnitOfWorkStorageProvider.Add(this);
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        public void Commit()
        {
            if (Transaction == null)
                throw new ObjectDisposedException(GetType().Name);

            DbContext.SaveChanges();
            Transaction.Commit();
            Commited = true;
        }

        public async Task CommitAsync()
        {
            if (Transaction == null)
                throw new ObjectDisposedException(GetType().Name);

            await DbContext.SaveChangesAsync();
            await Transaction.CommitAsync();
            Commited = true;
        }

        /// <summary>
        /// Rollback all changes made in this context to the database.
        /// </summary>
        public void Rollback()
        {
            if (Transaction == null)
                return;

            try
            {
                Transaction.Rollback();
            }
            finally
            {
                Transaction.Dispose();
                Transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (Transaction == null)
                return;

            try
            {
                await Transaction.RollbackAsync();
            }
            finally
            {
                await Transaction.DisposeAsync();
                Transaction = null;
            }
        }

        // <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">The disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (!Commited)
                            Rollback();
                    }
                    finally
                    {
                        UnitOfWorkStorageProvider.Remove(this);
                    }
                }
            }

            disposed = true;
        }
    }
}
