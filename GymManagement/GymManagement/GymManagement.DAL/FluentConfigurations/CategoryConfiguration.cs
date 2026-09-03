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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(C => C.CategoryName)
                   .HasColumnType("varchar")
                   .HasMaxLength(20);

            builder.HasData(
               new Category { Id=1, CategoryName="Cardio" },
                    new Category { Id=2, CategoryName="Strength" },
                    new Category { Id=3, CategoryName="Yoga" },
                    new Category { Id=4, CategoryName="Boxing" },
                    new Category { Id=5, CategoryName="CrossFit" }
                   );
        }
    }
}
