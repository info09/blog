using BlogCMS.Core.Domain.Identity;
using BlogCMS.Core.SeedWorks;
using BlogCMS.Core.SeedWorks.Constants;
using BlogCMS.WebApp.Extensions;
using BlogCMS.WebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlogCMS.WebApp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(IUnitOfWork unitOfWork, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [Route("profile")]
        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUser();

            return View(new ProfileViewModel()
            {
                FirstName = user.FirstName,
                Email = user.Email,
                LastName = user.LastName
            });
        }

        [HttpGet("profile/edit")]
        public async Task<IActionResult> ChangeProfile()
        {
            var user = await GetCurrentUser();
            return View(new ChangeProfileViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
            });
        }

        [Route("/profile/edit")]
        [HttpPost]
        public async Task<IActionResult> ChangeProfile([FromForm] ChangeProfileViewModel model)
        {
            var user = await GetCurrentUser();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData[SystemConsts.FormSuccessMsg] = "Update profile thành công.";
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Update profile failed");

            }
            return View(model);
        }

        [Route("profile/change-password")]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Route("profile/change-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userProfile = await GetCurrentUser();

            var isPasswordValid = await _userManager.CheckPasswordAsync(userProfile, model.OldPassword);
            if (!isPasswordValid)
            {
                ModelState.AddModelError(string.Empty, "Old password is not correct");
                return View(model);
            }

            var result = await _userManager.ChangePasswordAsync(userProfile, model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(userProfile);
                TempData[SystemConsts.FormSuccessMsg] = "Change password successful";
                return Redirect(UrlConstants.Profile);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();

            return Redirect(UrlConstants.Home);
        }

        private async Task<AppUser> GetCurrentUser()
        {
            var userId = User.GetUserId();
            return await _userManager.FindByIdAsync(userId.ToString());
        }
    }
}
