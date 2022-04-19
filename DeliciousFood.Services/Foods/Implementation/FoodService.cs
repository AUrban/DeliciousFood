using AutoMapper;
using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.DataAccess.Providers;
using DeliciousFood.DataAccess.Repositories;
using DeliciousFood.Services.Base.Implementation;
using DeliciousFood.Services.Base.Model;
using DeliciousFood.Services.Exceptions;
using DeliciousFood.Services.Extensions;
using DeliciousFood.Services.Foods;
using DeliciousFood.Services.Foods.Model;
using DeliciousFood.Services.Security;
using DeliciousFood.Services.Users.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliciousFood.Services.Foods.Implementation
{
    /// <summary>
    /// Food service. 
    /// CRUD operations
    /// Getting number of calories from calories API if it's necessary
    /// </summary>
    public sealed class FoodService : EntityService<Food, FoodViewModel, FoodEditModel>, IFoodService
    {
        public ICaloriesProvider CaloriesProvider { get; set; }

        public IUserSessionProvider UserSessionProvider { get; set; }

        public IPolicyValidator PolicyValidator { get; set; }


        public IEntityRepository<User> UserRepository { get; set; }

        public User UserScope { get; set; }


        public IEntityRepository<UserDeliciousFood> UserDeliciousFoodRepository { get; set; }


        public FoodService(IMapper mapper,
                            IQueryableProvider queryableProvider, ICaloriesProvider caloriesProvider,
                            IUserSessionProvider userSessionProvider,
                            IPolicyValidator policyValidator,
                            IEntityRepository<Food> repository,
                            IEntityRepository<User> userRepository,
                            IEntityRepository<UserDeliciousFood> userDeliciousFoodRepository)
            : base(mapper, queryableProvider, repository)
        {
            CaloriesProvider = caloriesProvider;
            UserSessionProvider = userSessionProvider;
            PolicyValidator = policyValidator;
            UserRepository = userRepository;
            UserDeliciousFoodRepository = userDeliciousFoodRepository;
        }

        public async Task SetUserIdAsync(int userId)
        {
            if (!PolicyValidator.ValidateUserPolicyIntersect(Policy.AdminsPolicy) && userId != UserSessionProvider.UserId)
                throw new ServiceValidationException(nameof(userId), "A user id should be equal to logged user id!");

            UserScope = await UserRepository.GetAsync(userId);
            if (UserScope == null)
                throw new ServiceNotFoundException(typeof(User), nameof(userId), userId);
        }

        public async Task<List<FoodViewModel>> GetAllAsync(FilterModel requestModel)
        {
            var repo = PolicyValidator.ValidateUserPolicyIntersect(Policy.AdminsPolicy)
                ? base.GetRepository()
                : UserRepository.GetSubRepository<Food>(await UserRepository.GetAsync(UserSessionProvider.UserId));
            return (await QueryableProvider.MaskAsyncListFromQuery(ApplyFilter(repo.UntrackedQuery, requestModel)))
                                           .Select(e => ModelToViewModel(e)).ToList();
        }

        public async Task<List<FoodViewModel>> GetPublicRecordsAsync(FilterModel requestModel)
        {
            var query = base.GetRepository().UntrackedQuery.Where(e => e.IsPublic);
            return (await QueryableProvider.MaskAsyncListFromQuery(ApplyFilter(query, requestModel)))
                                           .Select(e => ModelToViewModel(e)).ToList();
        }

        public async Task<FoodViewModel> MarkAsDeliciousAsync(UserDeliciousFoodEditModel editModel)
        {
            var food = await base.GetRepository().GetAsync(editModel.FoodId);
            if (food == null)
                throw new ServiceNotFoundException(typeof(Food), nameof(editModel.FoodId), editModel.FoodId);
            if (!food.IsPublic)
                throw new ServiceValidationException(nameof(editModel.FoodId), $"Food with id = {editModel.FoodId} is not public!");

            var user = await UserRepository.GetAsync(UserSessionProvider.UserId);
            var subRepository = UserRepository.GetSubRepository<UserDeliciousFood>(user);
            var countryFood = subRepository.Query.FirstOrDefault(e => e.Food.Country == food.Country);
            if (countryFood != null)
                await subRepository.DeleteAsync(countryFood);

            var deliciousFood = subRepository.Create();
            deliciousFood.FoodId = editModel.FoodId;
            await subRepository.SaveAsync(deliciousFood);
            return ModelToViewModel(food);
        }

        public async Task<List<FoodViewModel>> GetDeliciousFoodsAsync(FilterModel requestModel)
        {
            var user = await UserRepository.GetAsync(UserSessionProvider.UserId);
            var subRepository = UserRepository.GetSubRepository<UserDeliciousFood>(user);
            return (await QueryableProvider.MaskAsyncListFromQuery(ApplyFilter(subRepository.UntrackedQuery.Select(e => e.Food), requestModel)))
                                          .Select(e => ModelToViewModel(e)).ToList();
        }


        protected override IEntityRepository<Food> GetRepository()
        {
            return UserRepository.GetSubRepository<Food>(UserScope);
        }

        protected override FoodViewModel ModelToViewModel(Food model, FoodViewModel viewModel = null)
        {
            var result = base.ModelToViewModel(model, viewModel);
            result.UserDescription = UserRepository.Query.First(x => x.Id == result.UserId).Name;
            result.TypeDescription = result.Type.GetName();
            return result;
        }

        protected override async Task<Food> SaveModelAsync(FoodEditModel editModel)
        {
            if (!editModel.NumberOfCalories.HasValue || editModel.NumberOfCalories == 0)
                editModel.NumberOfCalories = await GetCaloriesByFoodAsync(editModel.Title);

            return await base.SaveModelAsync(editModel);
        }

        protected override async Task<Food> UpdateModelAsync(int id, FoodEditModel editModel)
        {
            if (editModel.Id.HasValue && editModel.Id != id)
                throw new ServiceValidationException(nameof(editModel.Id), "When altering an existing record, id must not be empty!");
            editModel.Id = id; // if editModel.id is null
            Validate(editModel);

            var food = await GetModelAsync(id);
            if (food.IsPublic && !editModel.IsPublic)
            {
                var subRepository = Repository.GetSubRepository<UserDeliciousFood>(food);
                if (subRepository.Query.Count() > 0)
                    throw new ServiceValidationException(nameof(editModel.IsPublic),
                        "The food cannot be edited to non-public, because it is referenced by the users' delicious records!");
            }

            if (!editModel.NumberOfCalories.HasValue || editModel.NumberOfCalories == 0)
                editModel.NumberOfCalories = await GetCaloriesByFoodAsync(editModel.Title);

            return await base.UpdateModelAsync(id, editModel);
        }

        protected override async Task<Food> DeleteModelAsync(int id)
        {
            var food = await GetModelAsync(id);
            var subRepository = Repository.GetSubRepository<UserDeliciousFood>(food);
            if (subRepository.Query.Count() > 0)
                throw new ServiceValidationException(nameof(Food), "The food cannot be deleted, because it is referenced by the users' delicious records!");

            return await base.DeleteModelAsync(id);
        }

        /// <summary>
        /// Getting number of calories from calories API
        /// </summary>
        private async Task<decimal> GetCaloriesByFoodAsync(string title)
        {
            try
            {
                return await CaloriesProvider.GetNumberOfCaloriesAsync(title);
            }
            catch
            {
                throw new ServiceValidationException(nameof(title), $"Bad request with food {title}");
            }
        }
    }
}
