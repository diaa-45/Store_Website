using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using STORE_Website.Models;
using STORE_Website.Services;
using STORE_Website.ViewModels;

namespace STORE_Website.Controllers
{
    [Authorize(Roles ="Admin,Manager")]
    public class ProductController : Controller
    {
        private readonly IReposirory<Product> productReposirory;
        private readonly IReposirory<Category> categoryRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IReposirory<Product> productReposirory, IReposirory<Category> categoryRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.productReposirory = productReposirory;
            this.categoryRepository = categoryRepository;
            this.webHostEnvironment = webHostEnvironment;
        }
        [AllowAnonymous]
        public IActionResult Index(string name = "",decimal? minPrice = null, decimal? maxPrice = null,int? quantity=null)
        {
            var products = productReposirory.GetAll();
            if (!String.IsNullOrWhiteSpace(name))
            {
                products= products.Where(p => p.Name.Contains(name,StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value).ToList();
            }
            if (quantity.HasValue)
            {
                products = products.Where(p => p.Stock >= quantity.Value).ToList();
            }
            return View("Index",products);
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories= categoryRepository.GetAll();
            return View("Create");
        }
        [HttpPost]
        public async Task<IActionResult> SaveCreate(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (productViewModel.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images/products");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + productViewModel.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await productViewModel.ImageFile.CopyToAsync(fileStream);
                    }
                }
                
                var product = new Product
                {
                    Name = productViewModel.Name,
                    Description = productViewModel.Description,
                    Price = productViewModel.Price,
                    Stock = productViewModel.Stock,
                    CategoryId = productViewModel.CategoryId,
                    ImageUrl = uniqueFileName != null ? "/images/products/" + uniqueFileName : null
                };

                await productReposirory.Create(product);
                return RedirectToAction("Index");
            }
            return View(productViewModel);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var product = await productReposirory.GetOne(id);
            ViewBag.category = await categoryRepository.GetOne(product.CategoryId);
            return View("Details",product);
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            ViewBag.Categories = categoryRepository.GetAll();
            return View("Update",await productReposirory.GetOne(id));
        }
        public async Task<IActionResult> SaveUpdate(UpdateProductViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
                Product product = await productReposirory.GetOne(id);
                if(product!=null)
                {
                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.Price = model.Price;
                    product.Stock = model.Stock;
                    product.CategoryId = model.CategoryId;

                    await productReposirory.Update(product);
                
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Categories =  categoryRepository.GetAll();
            return View("Update",model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Product product = await productReposirory.GetOne(id);
            ViewBag.category = await categoryRepository.GetOne(product.CategoryId);
            return View("Delete", product);
        }
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            Product product = await productReposirory.GetOne(id);
            productReposirory.Delete(product);
            productReposirory.Save();
            return RedirectToAction("Index");
        }
    }
}
