using DeliciousFood.DataAccess.DataModels.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliciousFood.DataAccess.DatabaseContext.Configuration
{
    /// <summary>
    /// Basic entity configuration for the data model in the database
    /// </summary>
    public abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T>
        where T : class, IEntity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(e => e.Id);
            builder
                .Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();
        }
    }
}
