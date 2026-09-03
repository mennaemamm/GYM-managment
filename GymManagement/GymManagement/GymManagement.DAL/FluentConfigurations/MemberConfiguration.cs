using GymManagement.DAL.FluentConfigurations;
using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Configurations
{
    public class MemberConfiguration : GymUserConfiguration<Member>, IEntityTypeConfiguration<Member>
    {
        public new void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.Property(M => M.CreatedAt)
                   .HasColumnName("JoinDate")
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(m => m.User)
                    .WithOne()
                    .HasForeignKey<Member>(m => m.UserId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);

            base.Configure(builder);
        }
    }
}
