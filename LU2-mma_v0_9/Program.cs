using Microsoft.EntityFrameworkCore;
using LU2_software_testen.Services;
using LU2_software_testen.Models;
using System.IO;
using log4net;
using log4net.Config;
using System.Reflection;
// Get the solution base directory (3 levels up from output: bin/Debug/net9.0)
var baseDir = AppContext.BaseDirectory;
var solutionDir = Directory.GetParent(baseDir)!.FullName;

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var log = LogManager.GetLogger(typeof(Program));
log.Info("Log4Net is geconfigureerd en de applicatie start nu op...");

// Ensure Logs directory exists at the solution root
var logsPath = Path.Combine(solutionDir, "Logs");
if (!Directory.Exists(logsPath))
{
    Directory.CreateDirectory(logsPath);
}

// Ensure today's log file exists in the solution root Logs directory
var logFileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
var logFilePath = Path.Combine(logsPath, logFileName);
if (!File.Exists(logFilePath))
{
    File.Create(logFilePath).Dispose(); // Dispose to release the file handle
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add MySQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0))
    ));

// Register your custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();

// Add cookie authentication only (no Identity)
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var logPath = builder.Configuration["Logging:LogPath"] 
              ?? logsPath;
builder.Services.AddSingleton(new LogPathOptions { LogPath = logPath });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession(); // Enable session middleware

// Custom middleware: Redirect patient to MyPrescriptions
app.Use(async (context, next) =>
{
    // Only redirect if user is authenticated, has "patient" role, and is not already on MyPrescriptions or logging out
    var user = context.User;
    var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
    if (user.Identity?.IsAuthenticated == true
        && user.IsInRole("patient")
        && !path.Contains("/prescriptions/myprescriptions")
        && !path.Contains("/account/logout")
        && !path.Contains("/account/login"))
    {
        context.Response.Redirect("/Prescriptions/MyPrescriptions");
        return;
    }
    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
