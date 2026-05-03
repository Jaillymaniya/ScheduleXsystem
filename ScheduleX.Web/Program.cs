using ScheduleX.Web.Components;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using ScheduleX.Core.Entities;
using Microsoft.AspNetCore.Components.Authorization;
using ScheduleX.Core.Interfaces;
using ScheduleX.Infrastructure.Repositories;
using ScheduleX.Web.Services.Admin;
using Timetable.Infrastructure.Repositories;
using ScheduleX.Core.Interfaces.TTCoordinator;

var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ================= BLAZOR =================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

// ================= IDENTITY =================
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ================= AUTH =================


builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();

// 🔥 REQUIRED FOR BLAZOR
builder.Services.AddCascadingAuthenticationState();


// ================= COOKIE =================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/login";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// ================= API =================
builder.Services.AddControllers();

// ================= HTTP CLIENT =================
builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri(nav.BaseUri)
    };
});

// ================= SERVICES =================
builder.Services.AddHttpContextAccessor();


// ================= SERVICES =================
builder.Services.AddScoped<EmailService>();


builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<DepartmentApiService>();
builder.Services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
builder.Services.AddScoped<AcademicYearApiService>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<CourseApiService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

await SeedData(app);

// ================= MIDDLEWARE =================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseExceptionHandler("/error");
app.UseStatusCodePagesWithRedirects("/404");

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();


// 🔥 IMPORTANT
app.MapControllers();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// ================= SEED =================
async Task SeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        // ROLES
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole<int>("Admin"));

        if (!await roleManager.RoleExistsAsync("TTCoordinator"))
            await roleManager.CreateAsync(new IdentityRole<int>("TTCoordinator"));

        // ADMIN
        var admin = await userManager.FindByNameAsync("admin");

        if (admin == null)
        {
            var newAdmin = new User
            {
                UserName = "admin",
                Email = "admin@schedulex.com",
                FullName = "System Admin",
                Role = UserRole.Admin,
                EmailConfirmed = true,
                IsActive = true,
                PhoneNumber = "9999999999"
            };

            var result = await userManager.CreateAsync(newAdmin, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
                Console.WriteLine("✅ Admin created");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Seeder error: " + ex.Message);
    }
}