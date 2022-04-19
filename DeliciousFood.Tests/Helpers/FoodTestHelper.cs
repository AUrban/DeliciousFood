using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.Enumerations;
using DeliciousFood.Services.Foods.Model;
using System;
using Xunit;

namespace DeliciousFood.Tests.UnitTests.Helpers
{
    public static class FoodTestHelper
    {
        public static void AssertFoodViewModels(FoodViewModel expected, FoodViewModel actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Type, actual.Type);            
            Assert.Equal(expected.NumberOfCalories, actual.NumberOfCalories);
            Assert.Equal(expected.Country, actual.Country);
            Assert.Equal(expected.IsPublic, actual.IsPublic);
        }

        public static void AssertFoodEditModels(FoodEditModel expected, FoodEditModel actual, bool ignoreId = false)
        {
            if (!ignoreId)
                Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Type, actual.Type);
            Assert.Equal(expected.NumberOfCalories, actual.NumberOfCalories);
            Assert.Equal(expected.Country, actual.Country);
            Assert.Equal(expected.IsPublic, actual.IsPublic);
        }

        public static void AssertFoodViewEditModels(FoodEditModel expected, FoodViewModel actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Type, actual.Type);
            Assert.Equal(expected.NumberOfCalories, actual.NumberOfCalories);
            Assert.Equal(expected.Country, actual.Country);
            Assert.Equal(expected.IsPublic, actual.IsPublic);
        }

        public static Food GetRandomFood(int id, int userId, bool? isPublic = null, string country = "USA")
        {
            return new Food
            {
                Id = id,
                UserId = userId,
                Title = new Random().Next().ToString(),
                Type = (FoodType)new Random().Next(4),
                Country = country,
                IsPublic = isPublic ?? false
            };
        }

        public static Food GetFood(int id, int userId, string title, FoodType type, int calories, string country, bool isPublic)
        {
            return new Food
            {
                Id = id,
                UserId = userId,
                Title = title,
                Type = type,
                NumberOfCalories = calories,
                Country = country,
                IsPublic = isPublic
            };
        }

        public static FoodViewModel GetFoodViewModel(Food food)
        {
            return new FoodViewModel
            {
                Id = food.Id,
                UserId = food.UserId,
                Title = food.Title,
                Type = food.Type,
                NumberOfCalories = food.NumberOfCalories,
                Country = food.Country,
                IsPublic = food.IsPublic
            };
        }

        public static FoodEditModel GetFoodEditModel(Food food)
        {
            return new FoodEditModel
            {
                Id = food.Id,
                Title = food.Title,
                Type = food.Type,
                NumberOfCalories = food.NumberOfCalories,
                Country = food.Country,
                IsPublic = food.IsPublic
            };
        }
    }
}
