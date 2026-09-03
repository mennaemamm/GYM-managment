using GymManagement.BLL.Profiles;
using GymManagement.BLL.Services.AttachmentService;
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Context;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();

builder.Services.AddAutoMapper(X => X.AddProfile(new MappingProfile()));

builder.Services.AddDbContext<GymDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(Config =>
{
    //Config.Password.RequiredLength = 6;
    //Config.Password.RequireLowercase = true;
    //Config.Password.RequireUppercase = true;
    Config.User.RequireUniqueEmail = true;
    Config.Lockout.MaxFailedAccessAttempts = 5;
    Config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);

}).AddEntityFrameworkStores<GymDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // redirect unauthenticated users (401)
    options.LoginPath = "/Account/Login";
    // redirect forbidden users (403)
    options.AccessDeniedPath = "/Account/AccessDenied";
});// Default Paths

var app = builder.Build();

await app.MigrateAndSeedDatabaseAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();