using DeliciousFood.DataAccess.DataModels.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliciousFood.DataAccess.DatabaseContext.Configuration
{
    /// <summary>
    /// Basic child entity configuration for the data model in the database
    /// </summary>
    public abstract class SubEntityConfiguration<T, TParent> : EntityConfiguration<T>
        where T : class, IEntity, ISubEntity<TParent>
        where TParent : class, IEntity
    {
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Ignore(b => b.Parent);
        }
    }
}
