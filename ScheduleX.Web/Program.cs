using ScheduleX.Web.Components;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;
using Microsoft.AspNetCore.Components;

//using ScheduleX.Core.Interfaces;
//using ScheduleX.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// ================= DB =================
// ✅ ONLY FACTORY (IMPORTANT)
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
