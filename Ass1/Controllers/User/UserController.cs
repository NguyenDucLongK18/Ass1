using Ass1.Models;
using Ass1.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ass1.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Ass1.Controllers.User
{
    public class UserController : Controller
    {
        private readonly UserManagerService _authService;

        private readonly IConfiguration _configuration;

        public UserController(UserManagerService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _authService.LoginAsync(model.Email, model.Password);

            if (user == null)
            {
                // Check if this is an Admin account
                var adminSection = _configuration.GetSection("Admin");

                if (adminSection.Exists())
                {
                    string adminEmail = adminSection.GetValue<string>("Username");
                    string adminPassword = adminSection.GetValue<string>("Password");
                    int adminRole = adminSection.GetValue<int>("UserRole");

                    if (!string.IsNullOrEmpty(adminEmail) && model.Email == adminEmail && model.Password == adminPassword)
                    {
                        user = new SystemAccount
                        {
                            AccountId = 0,
                            AccountName = "Admin",
                            AccountRole = adminRole
                        };
                    }
                }

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password");
                    return View(model);
                }
            }

            // Using Claims for user authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
                new Claim(ClaimTypes.Name, user.AccountName),
                new Claim(ClaimTypes.Role, user.AccountRole.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTime.UtcNow.AddDays(1) : null
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "1")]
        [HttpGet]
        public IActionResult Profile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var user = _authService.GetById(short.Parse(userIdClaim.Value));
            var userViewModel = new ProfileViewModel
            {
                AccountId = user.AccountId,
                AccountName = user.AccountName,
                AccountEmail = user.AccountEmail,
                AccountPassword = user.AccountPassword,
                SelectedRole = user.AccountRole ?? 2,
            };
            return View(userViewModel);
        }
    }
}
