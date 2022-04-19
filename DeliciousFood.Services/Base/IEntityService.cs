using DeliciousFood.Services.Base.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeliciousFood.Services.Base
{
    /// <summary>
    /// A basic service interface for CRUD operations over entities
    /// </summary>
    public interface IEntityService<TViewModel, TEditModel> : IService
        where TViewModel : class, IIdentifiedViewModel, new()
        where TEditModel : class, IIdentifiedEditModel, new()
    {
        /// <summary>
        /// Getting records by filter
        /// </summary>
        Task<List<TViewModel>> GetAsync(FilterModel requestModel);

        /// <summary>
        /// Getting a record by its id
        /// </summary>
        Task<TViewModel> GetByAsync(int id);

        /// <summary>
        /// Creation a record according to the edit model
        /// </summary>
        Task<TEditModel> SaveAsync(TEditModel editModel);

        /// <summary>
        /// Updating a record by its id according to the edit model
        /// </summary>
        Task<TEditModel> UpdateAsync(int id, TEditModel editModel);

        /// <summary>
        /// Deleting a record by its id
        /// </summary>
        Task<TViewModel> DeleteAsync(int id);
    }
}
