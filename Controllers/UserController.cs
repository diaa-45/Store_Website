using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STORE_Website.Models;
using STORE_Website.Services;
using STORE_Website.ViewModels;

namespace STORE_Website.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserController : Controller
    {
        private readonly IUserRepository reposirory;

        public UserController(IUserRepository reposirory)
        {
            this.reposirory = reposirory;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<ApplicationUser> users = await reposirory.GetAll();
            return View("Index", users);
        }
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            return View("Details",await reposirory.GetOne(id));
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            return View("Update", await reposirory.GetOne(id));
        }
        public async Task<IActionResult> SaveUpdate(ApplicationUser model, string id)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await reposirory.GetOne(id);
                if (user != null)
                {
                    user.FullName = model.FullName;
                    user.UserName = model.UserName;
                    user.City = model.City;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Address = model.Address;
                    
                    reposirory.Update(user);
                    reposirory.Save();
                    return RedirectToAction("Index");

                }
                return View("Update",model);
            }
            return View("Update", model);
        }
        public async Task<IActionResult> Delete(string id)
        {
            return View("Delete", await reposirory.GetOne(id));
        }
        public async Task<IActionResult> ConfirmDelete(string id)
        {
            ApplicationUser user = await reposirory.GetOne(id);
            reposirory.Delete(user);
            reposirory.Save();
            return RedirectToAction("Index");
        }

    }
}
