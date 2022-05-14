using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SweetsShop.Additionaly;
using SweetsShop.Data;
using SweetsShop.Models;
using SweetsShop.Models.Authorization;
using SweetsShop.Models.Client;
using SweetsShop.Models.ViewModels;
using System.Text.Json;

namespace SweetsShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {

            User user = await _db.Users.Include(u => u.AddressModel)
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            AccountVM AccountVM = new AccountVM()
            {
                Email = user.Email,
                Address = user.Address,
                AddressModel = user.AddressModel,
                Phone = user.Phone
            };
            return View(AccountVM);
        }
        [HttpGet]
        [Authorize]
        public IActionResult Details(string products,string returnUrl)  
        {
            ViewBag.returnUrl = returnUrl;
            List<ShoppingCart> shoppingCartList = JsonSerializer.Deserialize<List<ShoppingCart>>(products);
            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Products.Where(u => prodInCart.Contains(u.Id));
            List<ProductForUserOrder> ProductsForUserOrders = new List<ProductForUserOrder>();
            foreach (var obj in prodList)
            {
                ProductForUserOrder productForUserOrder = new ProductForUserOrder();
                productForUserOrder.Product = obj;
                productForUserOrder.Count = shoppingCartList.FirstOrDefault(u => u.ProductId == obj.Id).Count;
                ProductsForUserOrders.Add(productForUserOrder);
            }
            return View(ProductsForUserOrders);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Orders(string returnUrl)
        {
            if (returnUrl != null) return Redirect(returnUrl);
            User user = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            IEnumerable<FullOrder> fullOrders = _db.Orders.Where(u => u.UserId==user.Id);
            return View(fullOrders);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(AccountVM AccountVM)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                AccountVM.Address = AccountVM.AddressModel.AddressModelToString();
                if (user.Address == null || user.Address != AccountVM.Address)
                {
                    user.Address = AccountVM.Address;
                    if(AccountVM.AddressModel.Id!=0) _db.AddressModels.Update(AccountVM.AddressModel);
                    else user.AddressModel = AccountVM.AddressModel;
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveAddress(int id)
        {
            User user = await _db.Users.Include(u=>u.AddressModel).FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            _db.AddressModels.Remove(user.AddressModel);
            user.Address = null;
            _db.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemovePhone(int id)
        {
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            user.Phone = null;
            _db.Update(user);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPhone(AccountVM AccountVM)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
                if (user.Phone == null || user.Phone != AccountVM.Phone)
                {
                    user.Phone = AccountVM.Phone;
                    _db.Update(user);
                    await _db.SaveChangesAsync();
                }
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    user = new User { Email = model.Email, PasswordHash = HashingPassword.HashPassword(model.Password),RoleId = 2};

                    _db.Users.Add(user);
                    await _db.SaveChangesAsync();
                    user.Role = new Role() {Name = "User"};
                    await Authenticate(user); // аутентификация
                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Пользователь с таким Email уже существует");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            if(returnUrl != null) ViewBag.returnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User user = await _db.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null && HashingPassword.VerifyHashedPassword(user.PasswordHash,model.Password))
                {
                    await Authenticate(user); // аутентификация

                    if (returnUrl == null) return RedirectToAction("Index", "Home");
                    else Redirect(returnUrl);
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(returnUrl);
        }
    }
}
