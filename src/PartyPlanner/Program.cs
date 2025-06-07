using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PartyPlanner.Data;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1.  Connection-string ophalen
//    • eerst environment-variable (prod)
//    • anders uit appsettings (dev)
// ------------------------------------------------------------
var conn = Environment.GetEnvironmentVariable(
              "ConnectionStrings__DefaultConnection")
          ?? builder.Configuration.GetConnectionString("DefaultConnection")
          ?? throw new InvalidOperationException(
                 "Connection string 'DefaultConnection' not found.");

// ------------------------------------------------------------
// 2.  DbContext registreren (Pomelo/MariaDB)
//    • ServerVersion.AutoDetect leest versie bij de 1e connect
//    • Connection resiliency: 3 retries, max 5 s tussenpogingen
// ------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(conn, 
        ServerVersion.AutoDetect(conn),
        mySqlOpt => 
        {
            mySqlOpt.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
            
            // Configure MySQL migrations history table
            mySqlOpt.MigrationsHistoryTable("__EFMigrationsHistory");
        }));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ------------------------------------------------------------
// 3.  Identity & authenticatie
// ------------------------------------------------------------
builder.Services.AddDefaultIdentity<IdentityUser>(opt =>
    {
        opt.SignIn.RequireConfirmedAccount = true;
        opt.Password.RequiredLength = 8;
        opt.Password.RequireNonAlphanumeric = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.Name = ".PartyPlanner.Auth";
    opt.LoginPath   = "/Identity/Account/Login";
    opt.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ------------------------------------------------------------
// 4.  HTTP-pipeline
// ------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();                 // live migraties
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // <- vóór Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
