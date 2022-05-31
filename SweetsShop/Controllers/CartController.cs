using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using SweetsShop.Additionaly;
using SweetsShop.Data;
using SweetsShop.Models;
using SweetsShop.Models.Client;
using SweetsShop.Models.ViewModels;
using SweetsShop.Services.Interfaces;

namespace SweetsShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailService _emailService;
        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment,IEmailService emailService)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailService = emailService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Any() != false)
            {
                //session exists
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);

            }
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Products.Where(u => prodInCart.Contains(u.Id));
            if (prodList.Any()) return View(prodList);
            else return RedirectToAction("Index", "Home");
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Any() != false)
            {
                //session exists
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);

            }
            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> RegistrationOrder()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            User user = await _db.Users.Include(u=>u.AddressModel).FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null &&
                HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Any() != false)
            {
                //session exists
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WC.SessionCart);

            }
            RegistrationOrderVM registrationVM = new RegistrationOrderVM()
            {
                Phone = user.Phone,
                AddressModel = user.AddressModel,
                prodList = shoppingCartList
            };
            return View(registrationVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrationOrder(RegistrationOrderVM RegistrationVM)
        {
            if (ModelState.IsValid || RegistrationVM.AddressModel.Id==0)
            {
                User user = await _db.Users
                    .FirstOrDefaultAsync(u => User.Identity.Name == u.Email);
                await _emailService.SendOrderToEmailAsync(User.Identity.Name, _webHostEnvironment.WebRootPath);
                FullOrder fullOrder = new FullOrder()
                {
                    Phone = RegistrationVM.Phone, Sum = RegistrationVM.Sum, Products = JsonSerializer.Serialize(RegistrationVM.prodList),
                    DateOrder = DateTime.Now, Address = RegistrationVM.AddressModel.AddressModelToString(), Comment = RegistrationVM.Comment,
                    UserId = user.Id
                };
                await _db.Orders.AddAsync(fullOrder);
                if (RegistrationVM.SaveAddress && fullOrder.Address!=user.Address)
                {
                    user.Address = fullOrder.Address;
                    if(RegistrationVM.AddressModel.Id!=0) _db.Update(RegistrationVM.AddressModel);
                    else user.AddressModel = RegistrationVM.AddressModel;
                }
                if (RegistrationVM.SavePhone) user.Phone = RegistrationVM.Phone;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }
            return View(RegistrationVM);
        }
    }
}
