using DeliciousFood.Common.Helpers;
using DeliciousFood.DataAccess.DataModels;
using DeliciousFood.DataAccess.DataModels.Base;
using DeliciousFood.DataAccess.Enumerations;
using Microsoft.EntityFrameworkCore;

namespace DeliciousFood.DataAccess.DatabaseContext
{
    /// <summary>
    /// A basic database context for EF ORM
    /// Setting data models conventions for database
    /// </summary>
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // context configuring
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedData(modelBuilder);
            ApplyFluentAPIConfiguration(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Set some default data in database
        /// </summary>
        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User[]
                {
                    new User
                    {
                        Id = 1,
                        Login = "admin",
                        PasswordHash = "$2a$11$WlpsmkBTfJkJhasuMvie0.XdjTgUPhRXcWbwJb5ywRB1Z/WrNJAyi",
                        Name = "Admin",
                        PolicyMask = Policy.AdminsPolicy
                    }
                });
        }

        // using Fluent API
        private void ApplyFluentAPIConfiguration(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Entity>();
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyHelper.GetCurrentAssembly(), (type) => !type.IsAbstract);
        }
    }
}
