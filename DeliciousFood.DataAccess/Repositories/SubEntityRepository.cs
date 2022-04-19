using DeliciousFood.DataAccess.DataModels.Base;
using DeliciousFood.DataAccess.Exceptions;
using DeliciousFood.DataAccess.Providers;
using DeliciousFood.DataAccess.Transactions;
using System;
using System.Linq;

namespace DeliciousFood.DataAccess.Repositories
{
    /// <summary>
    /// Implementation of the repository for CRUD operations over child entities in scope of Parent
    /// </summary>
    public class SubEntityRepository<T, TParent> : EntityRepository<T>, ISubEntityRepository<T, TParent>
        where T : class, IEntity, ISubEntity<TParent>
        where TParent : class, IEntity
    {
        public TParent Parent { get; }


        public SubEntityRepository(IUnitOfWorkStorageProvider unitOfWorkStorageProvider, IQueryableProvider queryableProvider,
                                   TParent parent) : 
            base(unitOfWorkStorageProvider,  queryableProvider)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(Parent));
        }

        /// <summary>
        /// Filtering query by Parent
        /// </summary>
        protected override IQueryable<T> TenantFilter(IQueryable<T> query)
        {
            if (Parent == null) 
                throw new ArgumentNullException(nameof(Parent));

            return QueryableProvider.MakeParentQuery(base.TenantFilter(query), typeof(TParent), Parent.Id);
        }

        /// <summary>
        /// Setting Parent to entity
        /// </summary>
        protected override T TenantSetter(T entity)
        {
            entity = base.TenantSetter(entity);
            entity.Parent = Parent;
            return entity;
        }

        /// <summary>
        /// Validate entity for Parent match
        /// </summary>
        protected override void TenantCheck(T entity)
        {
            base.TenantCheck(entity);
            if (entity.Parent.Id != Parent.Id)
                throw new DataAccessPermissionException($"The operation is not valid for the record id = {entity.Id} for the {typeof(TParent).Name} id = {Parent.Id}");
        }
    }
}
