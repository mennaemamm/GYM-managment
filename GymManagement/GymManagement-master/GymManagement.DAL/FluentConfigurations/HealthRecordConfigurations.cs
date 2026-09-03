using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagement.DAL.FluentConfigurations
{
    internal class HealthRecordConfigurations : IEntityTypeConfiguration<HealthRecord>
    {
        public void Configure(EntityTypeBuilder<HealthRecord> builder)
        {

            builder.Property(x => x.BloodType)
                .HasMaxLength(5);

            builder.Property(x => x.Note)
                   .HasMaxLength(500);
        }
    }
}
