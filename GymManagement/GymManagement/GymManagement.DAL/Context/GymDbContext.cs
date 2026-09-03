using GymManagement.Configurations;
using GymManagement.DAL.FluentConfigurations;
using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymManagement.DAL.Context
{
    public class GymDbContext : IdentityDbContext<ApplicationUser>
    {
        public GymDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(EB =>
            {
                EB.Property(X => X.FirstName)
                .HasColumnType("varchar")
                .HasMaxLength(50);

                EB.Property(X => X.LastName)
                .HasColumnType("varchar")
                .HasMaxLength(50);
            });
        }
        
        public DbSet<Plan> Plans { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}