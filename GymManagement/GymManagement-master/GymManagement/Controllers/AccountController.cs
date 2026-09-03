using GymManagement.BLL.ViewModels.AccountViewModels;
using GymManagement.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagement.Controllers
{
    public class AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<AccountController> logger) : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<AccountController> _logger = logger;

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || string.IsNullOrEmpty(user.UserName))
            {
                ModelState.AddModelError("InvalidLogin", "Invalid email or password.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} signed in.", user.Id);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User {UserId} is locked out.", user.Id);
                ModelState.AddModelError("InvalidLogin", "This account is temporarily locked. Try again later.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError("InvalidLogin", "Sign-in is not allowed for this account.");
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", "Invalid email or password.");
            }
            return View(model);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied() => View();

    }
}
