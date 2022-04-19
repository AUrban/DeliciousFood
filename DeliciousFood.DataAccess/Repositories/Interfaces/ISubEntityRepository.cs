using DeliciousFood.DataAccess.DataModels.Base;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// Typed repository interface for CRUD operations over child entities in scope of Parent
    /// </summary>
    public interface ISubEntityRepository<T, TParent> : IEntityRepository<T>
        where T : class, IEntity, ISubEntity<TParent>
        where TParent : class, IEntity
    {
        TParent Parent { get; }
    }
}
