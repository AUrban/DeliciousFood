using DeliciousFood.DataAccess.Repositories;
using DeliciousFood.DataAccess.Transactions;
using System;
using System.Data;
using System.Threading.Tasks;

namespace DeliciousFood.DataAccess
{
    /// <summary>
    /// A provider to perform datasourse (database) operation in scope of application transaction. Based on the unit of work pattern.
    /// </summary>
    public class DataAccessProvider : IDataAccessProvider
    {
        private readonly IUnitOfWorkStorageProvider UnitOfWorkStorageProvider;
        private readonly IUnitOfWorkFactory UnitOfWorkFactory;

        // checking if the transaction is not nested
        private bool IsTopMostTransaction => UnitOfWorkStorageProvider.Current == null;


        public DataAccessProvider(IUnitOfWorkStorageProvider unitOfWorkStorageProvider, IUnitOfWorkFactory unitOfWorkFactory)
        {
            UnitOfWorkStorageProvider = unitOfWorkStorageProvider;
            UnitOfWorkFactory = unitOfWorkFactory;
        }

        public void Run(Action action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (IsTopMostTransaction)
            {
                using var unitOfWork = UnitOfWorkFactory.CreateInstance(isolationLevel);
                action();
                unitOfWork.Commit();
            }
            else
                action();
        }

        public TResult Get<TResult>(Func<TResult> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (IsTopMostTransaction)
            {
                using var unitOfWork = UnitOfWorkFactory.CreateInstance(isolationLevel);
                var result = action();
                unitOfWork.Commit();
                return result;
            }
            else
                return action();
        }

        public async Task RunAsync(Func<Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (IsTopMostTransaction)
            {
                using var unitOfWork = UnitOfWorkFactory.CreateInstance(isolationLevel);
                await action();
                await unitOfWork.CommitAsync();
            }
            else
                await action();
        }

        public async Task<TResult> GetAsync<TResult>(Func<Task<TResult>> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (IsTopMostTransaction)
            {
                using var unitOfWork = UnitOfWorkFactory.CreateInstance(isolationLevel);
                var result = await action();
                await unitOfWork.CommitAsync();
                return result;
            }
            else
                return await action();
        }
    }
}
