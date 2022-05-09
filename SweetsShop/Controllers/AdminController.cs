using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SweetsShop.Data;
using SweetsShop.Models.Client;

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
            IEnumerable<FullOrder> fullOrders = _db.Orders;
            return View(fullOrders);
        }
    }
}
