using DeliciousFood.DataAccess.DataModels.Base;
using DeliciousFood.DataAccess.Providers;
using DeliciousFood.DataAccess.Transactions;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// Entity repository based on EF ORM
    /// </summary>
    public class EntityRepository<T> : EFRepository<T>, IEntityRepository<T> where T : class, IEntity
    {
        public EntityRepository(IUnitOfWorkStorageProvider unitOfWorkStorageProvider, IQueryableProvider queryableProvider) : 
            base(unitOfWorkStorageProvider, queryableProvider)
        {
        }

        public ISubEntityRepository<TChild, T> GetSubRepository<TChild>(T parent)
            where TChild : class, IEntity, ISubEntity<T>
        {
            return new SubEntityRepository<TChild, T>(UnitOfWorkStorageProvider, QueryableProvider, parent);
        }
    }
}
