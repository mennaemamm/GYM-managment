using GymManagement.DAL.Models;
using GymManagement.DAL.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagement.DAL.Context
{
    public static class UserDataSeeding
    {
        private class SeedMember
        {
            public string FirstName { get; set; } = default!;
            public string LastName { get; set; } = default!;
            public string Email { get; set; } = default!;
            public string Phone { get; set; } = default!;
            public DateOnly DateOfBirth { get; set; }
            public Gender Gender { get; set; }
            public Address Address { get; set; } = default!;
            public decimal Height { get; set; }
            public decimal Weight { get; set; }
            public string BloodType { get; set; } = default!;
        }

        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager,
                                            UserManager<ApplicationUser> userManager,
                                            GymDbContext dbContext,
                                            ILogger logger,
                                            CancellationToken ct = default)
        {
            try
            {
                bool HasUserRole = await roleManager.RoleExistsAsync("User");

                if (!HasUserRole)
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
                    if (!roleResult.Succeeded)
                        logger.LogError("Failed to create role {Role}: {Errors}", "User",
                            string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                }

                var RegularUsers = new List<SeedMember>()
                {
                    new SeedMember()
                    {
                        FirstName = "Hana",
                        LastName = "Tolba",
                        Email = "HanaTolba@gmail.com",
                        Phone = "01090517417",
                        DateOfBirth = new DateOnly(1999, 3, 14),
                        Gender = Gender.Female,
                        Address = new Address { BuildingNumber = 12, Street = "El Nasr St", City = "Cairo" },
                        Height = 165,
                        Weight = 58,
                        BloodType = "A+"
                    },
                    new SeedMember()
                    {
                        FirstName = "Shahd",
                        LastName = "Raafat",
                        Email = "ShahdRaafat@gmail.com",
                        Phone = "01551823259",
                        DateOfBirth = new DateOnly(2000, 7, 2),
                        Gender = Gender.Female,
                        Address = new Address { BuildingNumber = 5, Street = "Gamal Abdel Nasser St", City = "Giza" },
                        Height = 160,
                        Weight = 54,
                        BloodType = "O+"
                    },
                    new SeedMember()
                    {
                        FirstName = "Nada",
                        LastName = "Ata",
                        Email = "NadaAta@gmail.com",
                        Phone = "01157516484",
                        DateOfBirth = new DateOnly(1998, 11, 20),
                        Gender = Gender.Female,
                        Address = new Address { BuildingNumber = 27, Street = "El Thawra St", City = "Banha" },
                        Height = 168,
                        Weight = 62,
                        BloodType = "B+"
                    }
                };

                foreach (var seed in RegularUsers)
                {
                    // 1. Identity account
                    var user = await userManager.FindByEmailAsync(seed.Email);
                    if (user is null)
                    {
                        user = new ApplicationUser()
                        {
                            FirstName = seed.FirstName,
                            LastName = seed.LastName,
                            UserName = $"{seed.FirstName}{seed.LastName}",
                            Email = seed.Email,
                            PhoneNumber = seed.Phone
                        };

                        var createResult = await userManager.CreateAsync(user, "P@ssw0rd");
                        if (!createResult.Succeeded)
                        {
                            logger.LogError("Failed to create seed user {Email}: {Errors}", seed.Email,
                                string.Join("; ", createResult.Errors.Select(e => e.Description)));
                            continue;
                        }

                        var roleAssignResult = await userManager.AddToRoleAsync(user, "User");
                        if (!roleAssignResult.Succeeded)
                        {
                            logger.LogError("Failed to assign role 'User' to {Email}: {Errors}", seed.Email,
                                string.Join("; ", roleAssignResult.Errors.Select(e => e.Description)));
                            continue;
                        }

                        logger.LogInformation($"Seeded ApplicationUser {user.Email}");
                    }

                    // 2. Domain Member, linked via UserId, only if not already created
                    bool memberExists = await dbContext.Members.AnyAsync(m => m.UserId == user.Id, ct);
                    if (memberExists)
                        continue;

                    var member = new Member
                    {
                        Name = $"{seed.FirstName} {seed.LastName}",
                        Email = seed.Email,
                        Phone = seed.Phone,
                        DateOfBirth = seed.DateOfBirth,
                        Gender = seed.Gender,
                        Address = seed.Address,
                        UserId = user.Id,
                        HealthRecord = new HealthRecord
                        {
                            Height = seed.Height,
                            Weight = seed.Weight,
                            BloodType = seed.BloodType
                        }
                    };

                    dbContext.Members.Add(member);
                    await dbContext.SaveChangesAsync(ct);

                    logger.LogInformation($"Seeded Member {member.Name} linked to User {user.Email}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Regular user seeding failed.");
                throw;
            }
        }

    }
}
