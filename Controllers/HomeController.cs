using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using STORE_Website.Models;
using STORE_Website.Services;
using STORE_Website.ViewModels;
using System.Diagnostics;


namespace STORE_Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public readonly IReposirory<Category> categoryRepository;
        public readonly IReposirory<ApplicationUser> userRepository;
        public readonly IReposirory<Order> orderRepository;
        public readonly IReposirory<Product> productRepository;
        public readonly IReposirory<OrderItem> orderItemsRepository;



        public HomeController(ILogger<HomeController> logger,IReposirory<ApplicationUser> userRepository,
                                IReposirory<Order> orderRepository,IReposirory<Category> categoryRepository,
                                IReposirory<Product> productRepository,
                                IReposirory<OrderItem> orderItemsRepository)
        {
            _logger = logger;
            this.userRepository = userRepository;
            this.orderRepository = orderRepository;
            this.categoryRepository = categoryRepository;
            this.productRepository = productRepository;
            this.orderItemsRepository = orderItemsRepository;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                OrderCount = orderRepository.GetAll().Count(),
                UserCount =  userRepository.GetAll().Count(),
                CategoryCount = categoryRepository.GetAll().Count(),
                // Add other analytical data here as needed
            };
            ViewBag.ordersTotal = orderRepository.GetAll().Sum(o => o.TotalAmount);
			// Find the bestseller product
			var bestSeller = orderItemsRepository.GetAll().ToList()
				.GroupBy(i => i.ProductId) // Group by ProductId
				.Select(g => new
				{
					ProductId = g.Key,
					TotalQuantity = g.Sum(i => i.Quantity)
				})
				.OrderByDescending(p => p.TotalQuantity) // Order by total quantity descending
				.FirstOrDefault();

			ViewBag.bestSellerProduct = bestSeller != null
				? productRepository.GetAll().FirstOrDefault(p => p.Id == bestSeller.ProductId)
				: null;
            ViewBag.BestSellerNumberOfOrder =  bestSeller.TotalQuantity;
			ViewBag.Products = productRepository.GetAll().OrderBy(c => Guid.NewGuid()).Take(4).ToList();

            ViewBag.Categories = categoryRepository.GetAll();
            return View(viewModel);
        }
        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
