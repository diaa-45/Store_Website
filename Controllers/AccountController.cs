using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using STORE_Website.Models;
using STORE_Website.ViewModels;

namespace STORE_Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        public async Task<IActionResult> SaveRegister(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    UserName = model.UserName,
                    Address = model.Address,
                    City = model.City
                };

                IdentityResult result = await userManager.CreateAsync(appUser, model.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(appUser, "User");
                    await signInManager.SignInAsync(appUser, false);
                    return RedirectToAction("Index", controllerName: "Product");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View("Register", model);
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", controllerName: "Account");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View("SaveLogin");
        }

        [HttpPost]
        public async Task<IActionResult> SaveLogin(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user =await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var ValidPass= await userManager.CheckPasswordAsync(user, model.Password);
                    if (ValidPass)
                    {
                        await signInManager.SignInAsync(user, model.RemenberMe);
                        return RedirectToAction("Index", "Product");
                    }
                    ModelState.AddModelError("", "Email Or Password is wrong");
                    return View(model);
                }
                ModelState.AddModelError("", "Email Or Password is wrong");
                return View(model);
            }
            return RedirectToAction("Index", "Product");

        }
    }
}
