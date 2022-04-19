using DeliciousFood.DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliciousFood.DataAccess.DatabaseContext.Configuration
{
    /// <summary>
    /// RefreshToken configuration for the data model in the database
    /// </summary>
    public class RefreshTokenConfiguration : EntityConfiguration<RefreshToken>
    {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            base.Configure(builder);

            builder.ToTable("RefreshTokens");
            builder.Property(e => e.UserId)
                .IsRequired();
            builder.Property(e => e.Token)
                .IsRequired();
            builder.Property(e => e.LifeTime)
                .IsRequired();
            builder.Property(e => e.CreateTime)
                .IsRequired();
        }
    }
}
