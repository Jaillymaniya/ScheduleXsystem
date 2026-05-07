


// ================= USING =================

using ScheduleX.Web.Components;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using ScheduleX.Core.Entities;
using Microsoft.AspNetCore.Components.Authorization;

using ScheduleX.Core.Interfaces;
using ScheduleX.Core.Interfaces.Admin;
using ScheduleX.Core.Interfaces.TTCoordinator;

using ScheduleX.Infrastructure.Repositories;
using ScheduleX.Infrastructure.Repositories.Admin;

using ScheduleX.Web.Services.Admin;

using Timetable.Infrastructure.Repositories;
//using ScheduleX.Core.Interfaces.TTCoordinator;
//using ScheduleX.Infrastructure.Repositories.TT;

using ScheduleX.Web.Services;
using ScheduleX.Infrastructure.Repositories.TT;
using ScheduleX.Web.Services.TT;
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

    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }

        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
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

builder.Services.AddScoped<EmailService>();

builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<DepartmentApiService>();

builder.Services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
builder.Services.AddScoped<AcademicYearApiService>();

builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<CourseApiService>();

builder.Services.AddScoped<ISemesterRepository, SemesterRepository>();
builder.Services.AddScoped<SemesterApiService>();

builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<SubjectApiService>();

builder.Services.AddScoped<
    ITTCoordinatorRepository,
    TTCoordinatorRepository>();

builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
builder.Services.AddScoped<FacultyApiService>();
builder.Services.AddScoped<IDivisionRepository, DivisionRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

builder.Services.AddScoped<IRoomRepository, RoomRepository>();


builder.Services.AddScoped<ILectureConfigRepository, LectureConfigRepository>();
builder.Services.AddScoped<LectureConfigService>();

builder.Services.AddScoped<ChangePasswordService>();
//builder.Services.AddScoped<ITTCoordinatorRepository, TTCoordinatorRepository>();
//builder.Services.AddScoped<ITTCoordinatorService, TTCoordinatorService>();

builder.Services.AddScoped<
    ITTCoordinatorService,
    TTCoordinatorService>();

builder.Services.AddScoped<ProfileService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<SubjectApiService>();
builder.Services.AddScoped<TTSessionState>();
builder.Services.AddScoped<
    ISubjectSemesterRepository,
    SubjectSemesterRepository>();

builder.Services.AddScoped<
    SubjectSemesterApiService>();

// SUBJECT FACULTY
builder.Services.AddScoped<
    ISubjectFacultyRepository,
    SubjectFacultyRepository>();

builder.Services.AddScoped<
    SubjectFacultyApiService>();

var app = builder.Build();

// ================= SEED =================

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

// ================= MAP =================

app.MapControllers();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// ================= SEEDER =================

async Task SeedData(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;

    try
    {
        var userManager =
            services.GetRequiredService<UserManager<User>>();

        var roleManager =
            services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        // ADMIN ROLE
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(
                new IdentityRole<int>("Admin"));
        }

        // TT ROLE
        if (!await roleManager.RoleExistsAsync("TTCoordinator"))
        {
            await roleManager.CreateAsync(
                new IdentityRole<int>("TTCoordinator"));
        }

        // ADMIN USER
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

            var result =
                await userManager.CreateAsync(
                    newAdmin,
                    "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(
                    newAdmin,
                    "Admin");

                Console.WriteLine("Admin created");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}