using AutoMapper;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Providers;
using DeliciousFood.DataAccess.Repositories;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Security;
using DeliciousFood.Services.Base.Implementation;
using DeliciousFood.Services.Users.Model;
using DeliciousFood.Services.Exceptions;
using DeliciousFood.Services.Extensions;

namespace DeliciousFood.Services.Users.Implementation
{
    /// <summary>
    /// User service implementation: users CRUD operations
    /// </summary>
    public sealed class UserService : EntityService<User, UserViewModel, UserEditModel>, IUserService
    {
        private ISecurityProvider SecurityProvider { get; set; }

        private IPolicyValidator PolicyValidator { get; set; }


        public UserService(IMapper mapper,
                           IQueryableProvider queryableProvider,
                           IEntityRepository<User> repository,
                           ISecurityProvider securityProvider,
                           IPolicyValidator policyValidator)
            : base(mapper, queryableProvider, repository)
        {
            SecurityProvider = securityProvider;
            PolicyValidator = policyValidator;
        }

        protected override UserViewModel ModelToViewModel(User model, UserViewModel viewModel = null)
        {
            var result = base.ModelToViewModel(model, viewModel);
            result.PolicyDescription = string.Join(", ", GetPolicyList(result.PolicyMask).Select(e => e.GetName()));
            return result;
        }

        protected override User EditModelToModel(UserEditModel editModel, User model = null)
        {
            model = base.EditModelToModel(editModel, model);

            // policy
            model.PolicyMask = editModel.PolicyList.Select(e => e.Policy.Value).Aggregate((e1, e2) => e1 | e2);

            // password
            if (!string.IsNullOrEmpty(editModel.Password))
                model.PasswordHash = SecurityProvider.GetHashedPassword(editModel.Password);

            return model;
        }

        protected override UserEditModel ModelToEditModel(User model, UserEditModel editModel = null)
        {
            editModel = base.ModelToEditModel(model, editModel);
            editModel.PolicyList = GetPolicyList(model.PolicyMask).Select(e => new UserPolicyEditModel { Policy = e })
                                                                  .ToList();
            return editModel;
        }

        protected override async Task<User> SaveModelAsync(UserEditModel editModel)
        {
            if (string.IsNullOrEmpty(editModel.Password))
                throw new ServiceValidationException(nameof(editModel.Password), "When saving a new record, password must not be empty!");
            return await base.SaveModelAsync(editModel);
        }


        protected override void Validate(UserEditModel editModel)
        {
            base.Validate(editModel);

            // validate login
            if (RepositoryViewQuery.Any(x => x.Login == editModel.Login &&
                                         (!editModel.Id.HasValue || x.Id != editModel.Id)))
                throw new ServiceValidationException(nameof(editModel.Login), "A user with the same login already exists!");

            // validate password
            if (!string.IsNullOrEmpty(editModel.Password))
            {
                var complexity = SecurityProvider.CheckPasswordComplexity(editModel.Password);
                if (!string.IsNullOrEmpty(complexity))
                    throw new ServiceValidationException(nameof(editModel.Password), complexity);
            }
        }


        private IEnumerable<Policy> GetPolicyList(Policy policyMask)
        {
            return EnumerationExtensions.GetValues<Policy>()
                                        .Where(e => PolicyValidator.ValidatePolicyIntersect(policyMask, e));
        }
    }
}
