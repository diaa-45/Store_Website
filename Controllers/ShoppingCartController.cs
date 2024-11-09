using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STORE_Website.Models;
using STORE_Website.Services;
using System.Security.Claims;

namespace STORE_Website.Controllers
{
    [Authorize(Roles ="User")]
    public class ShoppingCartController : Controller
    {
        private readonly IReposirory<ShoppingCart> shoppingCartRepository;
        private readonly IReposirory<ShoppingCartItem> shoppingCartItemRepository;
        private readonly IReposirory<Product> productRepository;
        public ShoppingCartController(IReposirory<ShoppingCart> shoppingCartRepository, IReposirory<Product> productRepository,
                                       IReposirory<ShoppingCartItem> shoppingCartItemRepository)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
            this.shoppingCartItemRepository = shoppingCartItemRepository;
        }
        private async Task<ShoppingCart> GetOrCreateShoppingCartAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Assuming Username or UserId as unique identifier

            var cart =  shoppingCartRepository.GetAll().FirstOrDefault(c => c.ApplicationUserId == userId);
            if (cart == null)
            {
                cart = new ShoppingCart { ApplicationUserId = userId };
                await shoppingCartRepository.Create(cart);
            }
            return cart;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = await GetOrCreateShoppingCartAsync();
            var items =  shoppingCartItemRepository
                        .GetAll()
                        .AsQueryable()
                        .Where(i => i.ShoppingCartId == cart.Id)
                        .Include(i => i.Product)  // Include the related Product data if needed
                        .ToList();
            return View(items);
        }

        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await productRepository.GetOne(productId);
            if (product == null) return NotFound();

            var cart = await GetOrCreateShoppingCartAsync();

            var cartItem = (shoppingCartItemRepository
                                .GetAll().Where(i => i.ShoppingCartId == cart.Id && i.ProductId == productId))
                                .FirstOrDefault();

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    ShoppingCartId = cart.Id
                };
                await shoppingCartItemRepository.Create(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                await shoppingCartItemRepository.Update(cartItem);
            }

            return RedirectToAction("Index");
        }
    }
}
