using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("WebApplication1ContextConnection") ?? throw new InvalidOperationException("Connection string 'WebApplication1ContextConnection' not found.");

//builder.Services.AddDbContextPool<WebApplication1Context>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContextPool<WebApplication1Context>(options =>
{
    options.UseSqlServer(connectionString).LogTo(Console.WriteLine, LogLevel.Information);
    //options.UseLazyLoadingProxies();
});
builder.Services.AddIdentity<UserRegistration, IdentityRole>(options =>
{
    // Configure password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 4;
})
.AddEntityFrameworkStores<WebApplication1Context>()
.AddDefaultTokenProviders();
builder.Services.AddTransient<EmailService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Secure cookies
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Use secure cookies on HTTPS
});
builder.Services.AddSignalR();
// Cookie Authentication Configuration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set expiration for the cookie
            options.Cookie.IsEssential = true; // Make the cookie essential for the application
            options.Cookie.MaxAge = TimeSpan.FromMinutes(30); // This will ensure the cookie expires after 30 minutes of inactivity
            options.ReturnUrlParameter = "returnUrl";
        });

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication()
.AddGoogle(options =>
{
    options.ClientId = "198174372725-bc7vfgle3ko08m23djocgk397s82n8qe.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.CallbackPath = new PathString("/signin-google");

    // You can set other options as needed.
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Makes session cookie HttpOnly for security
    options.Cookie.IsEssential = true; // Essential cookie for sessions
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=PrimeCarDealsLandingPage}/{id?}");

app.Run();
