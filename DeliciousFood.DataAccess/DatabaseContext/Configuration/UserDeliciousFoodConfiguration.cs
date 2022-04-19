using DeliciousFood.DataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliciousFood.DataAccess.DatabaseContext.Configuration
{
    /// <summary>
    /// UserDeliciousFood configuration for the data model in the database
    /// </summary>
    public class UserDeliciousFoodConfiguration : SubEntityConfiguration<UserDeliciousFood, User>
    {
        public override void Configure(EntityTypeBuilder<UserDeliciousFood> builder)
        {
            base.Configure(builder);

            builder.ToTable("UserDeliciousFoods");
            builder.HasKey(e => new { e.UserId, e.FoodId });
            builder.HasOne(e => e.User)
                   .WithMany(e => e.DeliciousFoods)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Cascade)
                   .IsRequired();
            builder.HasOne(e => e.Food)
                   .WithMany(e => e.DeliciousFoods)
                   .HasForeignKey(e => e.FoodId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired();
        }
    }
}
