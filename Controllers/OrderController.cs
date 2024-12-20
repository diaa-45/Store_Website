﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using STORE_Website.Models;
using STORE_Website.Services;
using System.Security.Claims;

namespace STORE_Website.Controllers
{
    
    public class OrderController : Controller
    {
        private readonly IReposirory<ShoppingCart> shoppingCartRepository;
        private readonly IReposirory<ShoppingCartItem> shoppingCartItemRepository;
        private readonly IReposirory<Order> orderRepository;
        private readonly IReposirory<OrderItem> orderItemRepository;
        private readonly IReposirory<Product> productRepository;
        private readonly IReposirory<ApplicationUser> userRepository;


        public OrderController(IReposirory<ShoppingCart> shoppingCartRepository,
                                IReposirory<ShoppingCartItem> shoppingCartItemRepository,
                                IReposirory<Order> orderRepository,
                                IReposirory<OrderItem> orderItemRepository,
                                IReposirory<Product> productRepository,
                                IReposirory<ApplicationUser> userReposirory)
        {
            this.shoppingCartRepository = shoppingCartRepository;
            this.shoppingCartItemRepository = shoppingCartItemRepository;
            this.orderRepository = orderRepository;
            this.orderItemRepository = orderItemRepository;
            this.productRepository = productRepository;
            this.userRepository = userReposirory;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = orderRepository.GetAll().Where(o => o.UserId == userId).ToList();
            return View("Index",order);
        }
        public async Task<IActionResult> MakeOrder()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = shoppingCartRepository.GetAll().FirstOrDefault(c => c.ApplicationUserId == userId);
            var cartItem = shoppingCartItemRepository
                                .GetAll()
                                .AsQueryable()
                                .Where(i => i.ShoppingCartId == cart.Id)
                                .Include(i => i.Product)
                                .ToList();
            var product =  productRepository.GetAll().Where(p => p.Id == cartItem.FirstOrDefault().ProductId).ToList();
            decimal TotalPrice=0;
            Order order = new Order
            {   
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = "Confirmed"
            };
            await orderRepository.Create(order);
            orderRepository.Save();
            foreach (var item in cartItem)
            {
                OrderItem orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,
                    OrderId = order.Id
                };
                TotalPrice += (item.Quantity * item.Product.Price); 
                await orderItemRepository.Create(orderItem);
                orderItemRepository.Save();
            }

            order.TotalAmount = TotalPrice;
            await orderRepository.Update(order);
            // Clear the shopping cart after placing the order
            foreach(var item in cartItem)
            {
                shoppingCartItemRepository.Delete(item);
                shoppingCartItemRepository.Save();
            }
            return RedirectToAction("Index");

        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AllOrders(int page=1,int pageSize=10)
        {
            var orders = orderRepository.GetAll().ToList();
            int totalOrders = orders.Count();
            int totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;
            
            var orderPage = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.User=userRepository.GetAll().Where(u=>u.Id==orders.FirstOrDefault().UserId).ToList();


            return View("Index", orderPage);
        }

        [HttpGet]
        
        public async Task<IActionResult> Details(int id)
        {
            Order order = await orderRepository.GetOne(id);
            order.OrderItems = orderItemRepository.GetAll().Where(i => i.OrderId == order.Id).ToList();
            if (order.OrderItems.Count != 0)
            {
                ViewBag.Items = orderItemRepository.GetAll().Where(i => i.Id == order.OrderItems.FirstOrDefault().Id).ToList();
                ViewBag.Products = productRepository.GetAll().Where(p => p.Id == order.OrderItems.FirstOrDefault().ProductId).ToList();
                ViewBag.User = userRepository.GetAll().Where(u => u.Id == order.UserId).ToList();
            }
            return View("Details", order);
        }

    }
}
