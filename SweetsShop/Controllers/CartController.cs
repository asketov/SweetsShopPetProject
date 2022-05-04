using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SweetsShop.Additionaly;
using SweetsShop.Data;
using SweetsShop.Models;
using SweetsShop.Models.Authorization;
using SweetsShop.Models.Client;
using SweetsShop.Models.ViewModels;

namespace SweetsShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
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
            RegistrationVM registrationVM = new RegistrationVM()
            {
                Phone = user.Phone,
                AddressModel = user.AddressModel,
                prodList = shoppingCartList
            };
            return View(registrationVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrationOrder(RegistrationVM RegistrationVM)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.Include(u => u.AddressModel)
                    .FirstOrDefaultAsync(u => User.Identity.Name == u.Email);
                if (RegistrationVM.SaveAddress && (user.AddressModel.Id != RegistrationVM.AddressModel.Id ||
                                                   RegistrationVM.AddressModel.Id == 0))
                    user.AddressId = RegistrationVM.AddressModel.Id;
                if (RegistrationVM.SavePhone && user.Phone != RegistrationVM.Phone) user.Phone = RegistrationVM.Phone;
                    _db.Update(user);
                    FullOrder fullOrder = new FullOrder()
                    {
                        Phone = RegistrationVM.Phone,
                        AddressId = RegistrationVM.AddressModel.Id,

                    };
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(RegistrationVM);
        }
    }
}
