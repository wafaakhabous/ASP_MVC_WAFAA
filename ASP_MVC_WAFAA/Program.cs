using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ASP_MVC_WAFAA.Data;
using ASP_MVC_WAFAA.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog.Events;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Configurerr Serilog : capture des log events et les envoyer vers des destination
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Wafaalogg.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog(); // Use Serilog for logging
builder.Services.AddDbContext<ASP_MVC_WAFAAContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ASP_MVC_WAFAAContext") ?? throw new InvalidOperationException("Connection string 'ASP_MVC_WAFAAContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = new PathString("/Account/Login"); // Set your login page path
            options.AccessDeniedPath = new PathString("/Account/AccessDenied"); // Set your access denied page path
        });

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();  // Add console logger
    builder.AddDebug();    // Add debug logger
                           // You can add more loggers here (e.g., AddFile, AddEventLog, AddAzureWebAppDiagnostics, etc.)
});    // Add debug logger
builder.Services.AddDistributedMemoryCache(); // This is required to store session data in memory
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();  // Add console logger
    builder.AddDebug();    // Add debug logger
                           // You can add more loggers here (e.g., AddFile, AddEventLog, AddAzureWebAppDiagnostics, etc.)
});    // Add debug logger
builder.Services.AddDistributedMemoryCache(); // This is required to store session data in memory
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set your desired session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "debug",
    pattern: "debug/{controller=Debug}/{action=Index}");
app.MapControllerRoute(
    name: "account",
    pattern: "{controller=Account}/{action=Logout}/{id?}");
app.MapControllerRoute(
    name: "cart",
    pattern: "{controller=Cart}/{action=Cart}/{id?}");


app.Run();
