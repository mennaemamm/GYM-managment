using GymManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Ignore(b => b.Id);

            builder.HasKey(b => new { b.SessionId, b.MemberId });

            builder.Property(b => b.CreatedAt)
                   .HasColumnName("BookingDate")
                   .HasDefaultValueSql("GETDATE()");
        }
    }
}
