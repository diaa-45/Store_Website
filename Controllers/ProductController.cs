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
        public async Task<IActionResult> Index(string name = "",decimal? minPrice = null, decimal? maxPrice = null,int? quantity=null)
        {
            var products = await productReposirory.GetAll();
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
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories= await categoryRepository.GetAll();
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
                    Stock = productViewModel.Quantity,
                    CategoryId = productViewModel.CategoryId,
                    ImageUrl = uniqueFileName != null ? "/images/products/" + uniqueFileName : null
                };

                await productReposirory.Create(product);
                return RedirectToAction("Index");
            }
            return View(productViewModel);
        }

    }
}
