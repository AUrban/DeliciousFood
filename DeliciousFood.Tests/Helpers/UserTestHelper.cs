using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Users.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DeliciousFood.Tests.UnitTests.Helpers
{
    public static class UserTestHelper
    {
        public static void AssertUsers(User expected, User actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.PasswordHash, actual.PasswordHash);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.PolicyMask, actual.PolicyMask);
        }

        public static void AssertUserViewModels(UserViewModel expected, UserViewModel actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.PolicyMask, actual.PolicyMask);
        }

        public static void AssertUserEditModels(UserEditModel expected, UserEditModel actual, bool ignoreId = false)
        {
            if (!ignoreId)
                Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(GetPolicyMask(expected), GetPolicyMask(actual));
        }

        public static void AssertUserViewEditModels(UserEditModel expected, UserViewModel actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Login, actual.Login);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(GetPolicyMask(expected), actual.PolicyMask);
        }

        public static User GetUser(int id, string login = "login", string passwordHash = "passwordHash", string name = "name",
                                    Policy policy = Policy.AdminsPolicy)
        {
            return new User
            {
                Id = id,
                Login = $"{login}{id}",
                PasswordHash = $"{passwordHash}{id}",
                Name = $"{name}{id}",
                PolicyMask = policy
            };
        }

        public static UserViewModel GetUserViewModel(User user)
        {
            return new UserViewModel
            {
                Id = user.Id,
                Login = user.Login,
                Name = user.Name,
                PolicyMask = Policy.AdminsPolicy
            };
        }

        public static UserEditModel GetUserEditModelByParameters(int? id, string login = "login", string password = "password", string name = "name",
                                                           Policy policy = Policy.AdminsPolicy, int calories = 100)
        {
            return new UserEditModel
            {
                Id = id,
                Login = login,
                Password = password,
                Name = name,
                PolicyList = new List<UserPolicyEditModel> { new UserPolicyEditModel { Policy = policy } }
            };
        }

        public static UserEditModel GetUserEditModel(User user)
        {
            return new UserEditModel
            {
                Id = user.Id,
                Login = user.Login,
                Password = user.PasswordHash,
                Name = user.Name,
                PolicyList = new List<UserPolicyEditModel> { new UserPolicyEditModel { Policy = Policy.AdminsPolicy } }
            };
        }

        private static Policy GetPolicyMask(UserEditModel editModel)
        {
            return editModel.PolicyList.Select(e => e.Policy.Value).Aggregate((e1, e2) => (e1 | e2));
        }
    }
}
