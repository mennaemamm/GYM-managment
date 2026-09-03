using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(P => P.Name)
                .HasColumnType("varchar")
                .HasMaxLength(50);

            builder.Property(P => P.Description)
                .HasColumnType("varchar")
                .HasMaxLength(200);

            builder.Property(P => P.Price)
                .HasPrecision(10, 2);

            builder.Property(P => P.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("PlanDurationDaysCheck", "DurationDays Between 1 and 365");
            });

        }
    }
}
