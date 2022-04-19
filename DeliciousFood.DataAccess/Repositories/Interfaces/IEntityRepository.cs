using DeliciousFood.DataAccess.DataModels.Base;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// Typed repository interface for CRUD operations in terms of entity
    /// </summary>
    public interface IEntityRepository<T> : IRepository<T> where T : class, IEntity
    {
        ISubEntityRepository<TChild, T> GetSubRepository<TChild>(T parent) where TChild : class, IEntity, ISubEntity<T>;
    }
}
