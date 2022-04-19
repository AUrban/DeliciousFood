using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Linq.Dynamic.Core.Exceptions;
using DeliciousFood.DataAccess.Providers;
using DeliciousFood.DataAccess.Repositories;
using DeliciousFood.DataAccess.Extensions;
using DeliciousFood.DataAccess.DataModels.Base;
using DeliciousFood.Services.Base.Model;
using DeliciousFood.Services.Exceptions;

namespace DeliciousFood.Services.Base.Implementation
{
    /// <summary>
    /// A basic entity service containing all the methods for CRUD operations
    /// </summary>
    public class EntityService<TModel, TViewModel, TEditModel> : IEntityService<TViewModel, TEditModel>
        where TModel : class, IEntity, new()
        where TViewModel : class, IIdentifiedViewModel, new()
        where TEditModel : class, IIdentifiedEditModel, new()
    {
        protected IMapper Mapper { get; set; }

        protected IQueryableProvider QueryableProvider { get; set; }


        protected IEntityRepository<TModel> Repository { get; }


        public EntityService(IMapper mapper,
                             IQueryableProvider queryableProvider,
                             IEntityRepository<TModel> repository)
        {
            Mapper = mapper;
            QueryableProvider = queryableProvider;
            Repository = repository;
        }

        #region IEntityService implementation

        public async Task<List<TViewModel>> GetAsync(FilterModel requestModel)
        {
            // Getting domain models from IQueryable and mapping them into view models
            return (await QueryableProvider.MaskAsyncListFromQuery(ApplyFilter(RepositoryViewQuery, requestModel)))
                                           .Select(e => ModelToViewModel(e)).ToList();
        }

        public async Task<TViewModel> GetByAsync(int id)
        {
            return ModelToViewModel(await GetModelAsync(id));
        }

        public async Task<TEditModel> SaveAsync(TEditModel editModel)
        {
            return ModelToEditModel(await SaveModelAsync(editModel));
        }

        public virtual async Task<TEditModel> UpdateAsync(int id, TEditModel editModel)
        {
            return ModelToEditModel(await UpdateModelAsync(id, editModel));
        }

        public virtual async Task<TViewModel> DeleteAsync(int id)
        {
            return ModelToViewModel(await DeleteModelAsync(id));
        }

        #endregion // IEntityService implementation

        #region CRUD

        /// <summary>
        /// Applying given filter model to IQueryable
        /// </summary>
        protected virtual IQueryable<TModel> ApplyFilter(IQueryable<TModel> query, FilterModel filterModel)
        {
            try
            {
                if (filterModel == null)
                    throw new ServiceValidationException(nameof(FilterModel), "Filter model is invalid!");

                return QueryableProvider.MakeFilterQuery(query, filterModel.Filter)
                                        .ApplyPaging(filterModel.Skip ?? 0, filterModel.Limit);
            }
            catch (Exception ex)
            {
                if (ex is ParseException || ex is InvalidOperationException)
                    throw new ServiceValidationException(nameof(FilterModel), "Invalid query specified");

                throw;
            }
        }

        /// <summary>
        /// Getting a domain model by its id
        /// </summary>
        protected virtual async Task<TModel> GetModelAsync(int id)
        {
            var model = await GetRepository().GetAsync(id);
            if (model == null)
                throw new ServiceNotFoundException(typeof(TModel), nameof(id), id);
            return model;
        }

        /// <summary>
        /// Mapping from edit model to domain model and saving the domain model
        /// </summary>
        protected virtual async Task<TModel> SaveModelAsync(TEditModel editModel)
        {
            if (editModel.Id.HasValue)
                throw new ServiceValidationException(nameof(editModel.Id), "When saving a new record, id must be empty!");
            Validate(editModel);

            var model = EditModelToModel(editModel);
            await GetRepository().SaveAsync(model);
            return model;
        }

        /// <summary>
        /// Mapping from edit model to domain model and editing the domain model
        /// </summary>
        protected virtual async Task<TModel> UpdateModelAsync(int id, TEditModel editModel)
        {
            if (editModel.Id.HasValue && editModel.Id != id)
                throw new ServiceValidationException(nameof(id), "When altering an existing record, id route parameter must match the id parameter in the edit model!");
            editModel.Id = id; // if editModel.id is null
            Validate(editModel);

            var model = await GetModelAsync(id);
            model = EditModelToModel(editModel, model);
            await GetRepository().UpdateAsync(model);
            return model;
        }

        /// <summary>
        /// Deleting a domain model by its id
        /// </summary>
        protected virtual async Task<TModel> DeleteModelAsync(int id)
        {
            var model = await GetModelAsync(id);
            await GetRepository().DeleteAsync(model);
            return model;
        }

        #endregion // CRUD

        #region Repository

        /// <summary>
        /// Getting repository to access database domain models
        /// </summary>
        protected virtual IEntityRepository<TModel> GetRepository() => Repository;

        #endregion // Repository

        #region Query        

        /// <summary>
        /// IQueryable to get domain models to view
        /// </summary>
        protected IQueryable<TModel> RepositoryViewQuery => GetRepository().UntrackedQuery;

        /// <summary>
        /// IQueryable to get domain models to edit
        /// </summary>
        protected IQueryable<TModel> RepositoryEditQuery => GetRepository().Query;

        #endregion // Query

        #region Mapping

        /// <summary>
        /// Mapping from a domain model to a view model
        /// </summary>
        protected virtual TViewModel ModelToViewModel(TModel model, TViewModel viewModel = null)
        {
            if (viewModel == null)
                viewModel = Activator.CreateInstance<TViewModel>();
            return Mapper.Map(model, viewModel);
        }

        /// <summary>
        /// Mapping from a domain model to an edit model
        /// </summary>
        protected virtual TEditModel ModelToEditModel(TModel model, TEditModel editModel = null)
        {
            if (editModel == null)
                editModel = Activator.CreateInstance<TEditModel>();
            return Mapper.Map(model, editModel);
        }

        /// <summary>
        /// Mapping from an edit model to a domain model
        /// </summary>
        protected virtual TModel EditModelToModel(TEditModel editModel, TModel model = null)
        {
            if (model == null)
                model = GetRepository().Create();
            return Mapper.Map(editModel, model);
        }

        #endregion // Mapping

        #region Validate

        /// <summary>
        /// Validation edit model
        /// </summary>
        protected virtual void Validate(TEditModel editModel)
        {
        }

        #endregion // Validate
    }
}
