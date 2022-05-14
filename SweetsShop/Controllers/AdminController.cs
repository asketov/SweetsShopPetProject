using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SweetsShop.Data;
using SweetsShop.Models;
using SweetsShop.Models.Client;
using SweetsShop.Models.ViewModels;

namespace SweetsShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrdersForAdmin()
        {
            User user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            IEnumerable<FullOrder> fullOrders = _db.Orders.AsNoTracking();
            return View(fullOrders);
        }
        public async Task<IActionResult> Delete(int id)
        {
            FullOrder Order = new FullOrder()
            {
                Id = id
            };
            _db.Orders.Remove(Order);
            await _db.SaveChangesAsync();
            return RedirectToAction("OrdersForAdmin");
        }
        [HttpGet]
        public async Task<IActionResult> EditOrder(int id)
        {
            FullOrder Order = await _db.Orders.FirstOrDefaultAsync(u => u.Id == id);
            List<ShoppingCart> shoppingCartList = JsonSerializer.Deserialize<List<ShoppingCart>>(Order.Products);
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Products.Where(u => prodInCart.Contains(u.Id));
            List<ProductForUserOrder> ProductsForUserOrder = new List<ProductForUserOrder>();
            foreach (var obj in prodList)
            {
                ProductForUserOrder productForUserOrder = new ProductForUserOrder();
                productForUserOrder.Product = obj;
                productForUserOrder.Count = shoppingCartList.First(u => u.ProductId == obj.Id).Count;
                ProductsForUserOrder.Add(productForUserOrder);
            }

            EditOrderVM EditOrderVM = new EditOrderVM()
            {
                Comment = Order.Comment, Phone = Order.Phone, Address = Order.Address, ProductsForUserOrder = ProductsForUserOrder,OrderID = Order.Id
            };
            return View(EditOrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(EditOrderVM editOrderVM)
        {
            FullOrder Order = await _db.Orders.FirstOrDefaultAsync(u => u.Id == editOrderVM.OrderID);
            Order.Address = editOrderVM.Address; Order.Comment = editOrderVM.Comment;
            Order.Phone = editOrderVM.Phone; Order.Sum += editOrderVM.DifferenceInSum;
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            foreach (var obj in editOrderVM.ProductsForUserOrder)
            {
                if (obj.Count != 0)
                {
                    shoppingCartList.Add(new ShoppingCart() { ProductId = obj.Product.Id, Count = obj.Count });
                }
            }
            Order.Products = JsonSerializer.Serialize(shoppingCartList);
            _db.Orders.Update(Order);
            await _db.SaveChangesAsync();
            return RedirectToAction("OrdersForAdmin");
        }
    }
}
