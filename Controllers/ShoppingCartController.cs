using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using STORE_Website.Models;
using STORE_Website.Services;
using System.Security.Claims;

namespace STORE_Website.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ShoppingCartController : Controller
    {
        private readonly IReposirory<ShoppingCart> shoppingCartRepository;
        private readonly IReposirory<ShoppingCartItem> shoppingCartItemRepository;
        private readonly IReposirory<Product> productRepository;
        private readonly IReposirory<ApplicationUser> userReposirory;
        public ShoppingCartController(IReposirory<ShoppingCart> shoppingCartRepository, IReposirory<Product> productRepository,
                                       IReposirory<ShoppingCartItem> shoppingCartItemRepository,
                                       IReposirory<ApplicationUser> userReposirory)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.productRepository = productRepository;
            this.shoppingCartItemRepository = shoppingCartItemRepository;
            this.userReposirory = userReposirory;
        }
        
        [HttpGet]
        public async Task<IActionResult> AllCarts()
        {
            var carts = shoppingCartRepository.GetAll();
            if (carts.Count() != 0)
            {
                ViewBag.Users = userReposirory.GetAll().Where(u => u.Id == carts.FirstOrDefault().ApplicationUserId).ToList(); 
            }
            return View(carts);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            ShoppingCart cart = await shoppingCartRepository.GetOne(id);
            cart.Items= shoppingCartItemRepository.GetAll().Where(i => i.ShoppingCartId == cart.Id).ToList();
            if(cart.Items.Count != 0)
            {
                ViewBag.Items= shoppingCartItemRepository.GetAll().Where(i => i.Id == cart.Items.FirstOrDefault().Id).ToList();
                ViewBag.Products = productRepository.GetAll().Where(p => p.Id == cart.Items.FirstOrDefault().ProductId).ToList();

            }
            return View("Details", cart);
        }

        public async Task<IActionResult> Delete(int id)
        {
            ShoppingCart cart = await shoppingCartRepository.GetOne(id);
            shoppingCartRepository.Delete(cart);
            shoppingCartRepository.Save();
            return RedirectToAction("AllCarts");
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
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var cart = await GetOrCreateShoppingCartAsync();
            
            var items =  shoppingCartItemRepository
                        .GetAll()
                        .AsQueryable()
                        .Where(i => i.ShoppingCartId == cart.Id)
                        .Include(i => i.Product)  // Include the related Product data if needed
                        .ToList();
            if (items.Count() != 0)
            {
                ViewBag.Products =  productRepository.GetAll().Where(p => p.Id == items.FirstOrDefault().ProductId).ToList();
            }
            return View(items);
        }

        [AllowAnonymous]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await productRepository.GetOne(productId);
            if (product == null) return NotFound();

            var cart = await GetOrCreateShoppingCartAsync();

            var cartItem = shoppingCartItemRepository
                                .GetAll()
                                .AsQueryable()
                                .Include(i => i.Product)
                                .FirstOrDefault(i => i.ShoppingCartId == cart.Id && i.ProductId == productId);

            if (cartItem == null)
            {
                if(product.Stock==0) return RedirectToAction("Index");
                cartItem = new ShoppingCartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    ShoppingCartId = cart.Id
                };
                await shoppingCartItemRepository.Create(cartItem);
                cartItem.Product.Stock -= quantity;
                productRepository.Save();
            }
            else
            {
                if (product.Stock == 0) return RedirectToAction("Index");
                cartItem.Quantity += quantity;
                await shoppingCartItemRepository.Update(cartItem);
                cartItem.Product.Stock -= quantity;
                productRepository.Save();
            }

            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var cartItem = await shoppingCartItemRepository.GetOne(cartItemId);
            ViewBag.product = await productRepository.GetOne(cartItem.ProductId);
            cartItem.Product.Stock+= cartItem.Quantity;
            shoppingCartItemRepository.Delete(cartItem);
            shoppingCartItemRepository.Save();
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public async Task<IActionResult> IncreaseCartItem(int cartItemId)
        {
            var cartItem = await shoppingCartItemRepository.GetOne(cartItemId);
            ViewBag.product = await productRepository.GetOne(cartItem.ProductId);
            if (cartItem.Product.Stock!=0)
            {
                cartItem.Quantity +=1;
                cartItem.Product.Stock -= 1;
                await shoppingCartItemRepository.Update(cartItem);

            }
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public async Task<IActionResult> DecreaseCartItem(int cartItemId)
        {
            var cartItem = await shoppingCartItemRepository.GetOne(cartItemId);
            ViewBag.product = await productRepository.GetOne(cartItem.ProductId);
            if(cartItem.Quantity >1)
            {
                cartItem.Quantity -= 1;
                cartItem.Product.Stock += 1;
                await shoppingCartItemRepository.Update(cartItem);

            }
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public async Task<IActionResult> ClearCart(int cartId)
        {
            var cart = await shoppingCartRepository.GetOne(cartId);
            var cartItems = shoppingCartItemRepository.GetAll().Where(i => i.ShoppingCartId == cart.Id).ToList();
            foreach (var item in cartItems)
            {
                ViewBag.product = await productRepository.GetOne(item.ProductId);
                item.Product.Stock += item.Quantity;
                shoppingCartItemRepository.Delete(item);
            }
            shoppingCartItemRepository.Save();
            return RedirectToAction("Index");
        }
    }
}
