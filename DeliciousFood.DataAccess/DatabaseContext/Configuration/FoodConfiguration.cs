using DeliciousFood.DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliciousFood.DataAccess.DatabaseContext.Configuration
{
    public class FoodConfiguration : SubEntityConfiguration<Food, User>
    {
        public override void Configure(EntityTypeBuilder<Food> builder)
        {
            base.Configure(builder);

            builder.ToTable("Foods");
            builder.Property(e => e.UserId)
                .IsRequired();
            builder.Property(e => e.Title)
                .IsRequired();
            builder.Property(e => e.Type)
                .IsRequired();
            builder.Property(e => e.NumberOfCalories)
                .IsRequired()
                .HasColumnType("decimal(18, 2)");
            builder.Property(e => e.Country)
                .IsRequired();
            builder.Property(e => e.IsPublic)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}
