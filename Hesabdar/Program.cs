using Hesabdar.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=db/hesabdarDB.db"));

// Configure DbContext with SQL Server
builder.Services.AddDbContext<SqlServerDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServerConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; // مسیر صفحه لاگین
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // مدت اعتبار کوکی برای کاربران عادی
        options.SlidingExpiration = true; // تمدید اعتبار کوکی در صورت فعالیت
        options.Cookie.HttpOnly = true; // جلوگیری از دسترسی به کوکی از طریق جاوااسکریپت
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // فقط ارسال کوکی در هنگام استفاده از HTTPS
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Middleware احراز هویت
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
