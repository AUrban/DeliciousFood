using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.DataAccess.Providers;
using DeliciousFood.DataAccess.Transactions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// Implementation of the entity framework repository based on Entity Framework ORM
    /// </summary>
    public abstract class EFRepository<T> : IRepository<T> where T : class
    {
        protected IUnitOfWorkStorageProvider UnitOfWorkStorageProvider { get; }
        protected IQueryableProvider QueryableProvider { get; set; }

        
        protected IEFUnitOfWork EFUnitOfWork => UnitOfWorkStorageProvider.Current as IEFUnitOfWork;
        protected DbContext DbContext => EFUnitOfWork?.DbContext ?? throw new ArgumentNullException(nameof(EFUnitOfWork));
        protected DbSet<T> DbSet => DbContext.Set<T>();
        protected string TableName => DbContext.Model.FindEntityType(typeof(T)).GetTableName();
      

        protected EFRepository(IUnitOfWorkStorageProvider unitOfWorkStorageProvider, IQueryableProvider queryableProvider)
        {
            UnitOfWorkStorageProvider = unitOfWorkStorageProvider;
            QueryableProvider = queryableProvider;
        }

        #region IRepository

        public IQueryable<T> Query => TenantFilter(DbSet);

        public IQueryable<T> UntrackedQuery => TenantFilter(DbSet.AsNoTracking<T>());

        /// <summary>
        /// Filtering query
        /// </summary>
        protected virtual IQueryable<T> TenantFilter(IQueryable<T> query)
        {
            return query;
        }


        public virtual T Create()
        {
            return TenantSetter((T)Activator.CreateInstance(typeof(T)));
        }

        /// <summary>
        /// Setting default fields with scope parameters
        /// </summary>
        protected virtual T TenantSetter(T item)
        {
            return item;
        }


        public virtual T Get(int id)
        {
            var entity = DbSet.Find(id);
            CheckPermissions(entity, RepositoryOperation.Get);
            return entity;
        }

        public virtual async Task<T> GetAsync(int id)
        {
            var entity = await DbSet.FindAsync(id);
            CheckPermissions(entity, RepositoryOperation.Get);
            return entity;
        }


        public virtual void Save(T entity)
        {
            CheckPermissions(entity, RepositoryOperation.Save);
            DbSet.Add(entity);
            DbContext.SaveChanges();
        }

        public virtual async Task SaveAsync(T entity)
        {
            CheckPermissions(entity, RepositoryOperation.Save);
            await DbSet.AddAsync(entity);
            await DbContext.SaveChangesAsync();
        }


        public virtual void Update(T entity)
        {
            CheckPermissions(entity, RepositoryOperation.Update);
            DbSet.Update(entity);
            DbContext.SaveChanges();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            CheckPermissions(entity, RepositoryOperation.Update);
            DbSet.Update(entity);
            await DbContext.SaveChangesAsync();
        }


        public virtual void Delete(T entity)
        {
            CheckPermissions(entity, RepositoryOperation.Delete);
            DbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            CheckPermissions(entity, RepositoryOperation.Delete);
            DbSet.Remove(entity);
            await DbContext.SaveChangesAsync();
        }


        public virtual void Dispose()
        {
        }

        #endregion // IRepository

        #region Util methods

        /// <summary>
        /// Validate entity and repository operation
        /// </summary>
        protected virtual void CheckPermissions(T entity, RepositoryOperation operation)
        {
            if (entity != null)
                TenantCheck(entity);
        }

        /// <summary>
        /// Validate entity to scope parameters
        /// </summary>
        protected virtual void TenantCheck(T item)
        {
        }

        #endregion // Util methods
    }
}
