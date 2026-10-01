using Hesabdar.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using Hesabdar.Utility;

namespace Hesabdar.Controllers
{
    public class UserController : Controller
    {
        private readonly SqlServerDbContext _sqlServerDbContext;
        public UserController(SqlServerDbContext sqlServerDbContext)
        {
            _sqlServerDbContext = sqlServerDbContext;
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CheckLogin(string email, string password, bool rememberMe)
        {
            // رمز عبور ورودی را هش می‌کنیم
            string newPass = General.EncodePasswordMd5(password);

            // بررسی وجود کاربر با ایمیل و رمز عبور هش‌شده
            var user = await _sqlServerDbContext.tbl_register.FirstOrDefaultAsync(u => u.email == email && u.password == newPass);

            if (user != null)
            {
                // کاربر یافت شد و احراز هویت موفقیت‌آمیز بود
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // تنظیمات کوکی
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe, // اگر rememberMe تیک خورده باشد، کوکی پایدار خواهد بود
                    ExpiresUtc = rememberMe
                        ? DateTimeOffset.UtcNow.AddDays(30)  // اعتبار کوکی برای کاربران با rememberMe فعال
                        : DateTimeOffset.UtcNow.AddMinutes(30) // اعتبار کوکی برای کاربران بدون rememberMe
                };

                // امضای کوکی و تنظیمات آن
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // در صورت نادرست بودن ایمیل یا رمز عبور
                ViewBag.ErrorMessage = "نام کاربری یا رمز عبور نادرست است.";
                return View("Login");
            }
        }

    }
}
