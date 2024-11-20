using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STORE_Website.Models;
using STORE_Website.Services;
using STORE_Website.ViewModels;

namespace STORE_Website.Controllers
{
    
    [Authorize(Roles ="Admin")]
    
    public class CategoryController : Controller
    {
        private readonly IReposirory<Category> reposirory;

        public CategoryController(IReposirory<Category> reposirory)
        {
            this.reposirory = reposirory;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Category> categories = reposirory.GetAll();
            return View("Index",categories);
        }
        
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            return View("Details",await reposirory.GetOne(id));
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }
        [HttpPost]
        public async Task<IActionResult> SaveCreate(CategoryViewModel categoryViewModel)
        {
            if(ModelState.IsValid)
            {
                Category category = new Category {
                    Name=categoryViewModel.Name,
                    Description=categoryViewModel.Description
                };
                await reposirory.Create(category);
                reposirory.Save();
                return RedirectToAction("Index");
            }
            return View(categoryViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            return View("Update",await reposirory.GetOne(id));
        }
        public async Task<IActionResult> SaveUpdate(CategoryViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
                Category category =await reposirory.GetOne(id);
                if (category != null)
                {
                    category.Name = model.Name;
                    category.Description = model.Description;
                    await reposirory.Update(category);
                    reposirory.Save();
                    return RedirectToAction("Index");

                }
                return View(model);
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            return View("Delete",await reposirory.GetOne(id));
        }
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            Category category = await reposirory.GetOne(id);
            reposirory.Delete(category);
            reposirory.Save();
            return RedirectToAction("Index");
        }

    }
}
