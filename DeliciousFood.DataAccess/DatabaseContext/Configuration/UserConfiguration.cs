using DeliciousFood.DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliciousFood.DataAccess.DatabaseContext.Configuration
{
    /// <summary>
    /// User configuration for the data model in the database
    /// </summary>
    public class UserConfiguration : EntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.ToTable("Users");
            builder.Property(e => e.Login)
                .IsRequired();
            builder.Property(e => e.PasswordHash)
                .IsRequired();
            builder.Property(e => e.Name)
                .IsRequired();
            builder.Property(e => e.PolicyMask)
                .IsRequired();
        }
    }
}
