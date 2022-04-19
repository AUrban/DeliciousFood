using AutoMapper;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.DataAccess.Providers;
using DeliciousFood.Services.Base;
using DeliciousFood.Services.Security;
using NSubstitute;
using System.Linq;
using System.Threading.Tasks;

namespace DeliciousFood.Tests.UnitTests.Services
{
    public class BaseServiceTest
    {
        public BaseServiceTest()
        {
        }

        protected IMapper GetMapperMock()
        {
            return MapperOptions.ProvideMapper(null);
        }

        protected IQueryableProvider GetQueryableProviderMock<T>()
        {
            var queryableProvider = Substitute.For<IQueryableProvider>();
            queryableProvider.MakeFilterQuery(Arg.Any<IQueryable<T>>(), null).Returns(args => args.ArgAt<IQueryable<T>>(0));
            queryableProvider.MaskAsyncListFromQuery(Arg.Any<IQueryable<T>>()).Returns(args => Task.FromResult(args.ArgAt<IQueryable<T>>(0).ToList()));
            return queryableProvider;
        }

        protected IPolicyValidator GetPolicyValidator()
        {
            var policyValidator = Substitute.For<IPolicyValidator>();
            policyValidator.ValidatePolicyIntersect(Arg.Any<Policy>(), Arg.Any<Policy>())
                .Returns(args => (args.ArgAt<Policy>(0) & args.ArgAt<Policy>(1)) != Policy.None);
            return policyValidator;
        }
    }
}
