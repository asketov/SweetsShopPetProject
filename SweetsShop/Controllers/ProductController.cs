using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using SweetsShop.Data;
using SweetsShop.Models;
using SweetsShop.Models.ViewModels;

namespace SweetsShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db=db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Products;
            foreach (var obj in objList)
            {
                obj.Category = _db.Categories.FirstOrDefault(u=>u.Id==obj.CategoryId);
            }
            return View(objList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM() 
            {
                Product = new Product(),
                CategorySelectList = _db.Categories.Select(i => new SelectListItem()
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                //this is for create
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Products.Find(id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
            }
            return View(productVM);
        }

        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var obj = _db.Products.FirstOrDefault(u => u.Id == id);
            if (obj == null) return NotFound();
            _db.Products.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM ProductVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (ProductVM.Product.Id == 0)
                {
                    //create
                    string upload = webRootPath + WC.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    ProductVM.Product.Image = fileName + extension;
                    _db.Products.Add(ProductVM.Product);
                    _db.SaveChanges();
                }
                else
                {
                    //updating
                    var objFromDb = _db.Products.AsNoTracking().FirstOrDefault(u => u.Id == ProductVM.Product.Id);
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + WC.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (var fileStream =
                               new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        ProductVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        ProductVM.Product.Image = objFromDb.Image;
                    }

                    _db.Products.Update(ProductVM.Product);
                    _db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ProductVM.CategorySelectList = _db.Categories.Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(ProductVM);
        }
    }
}
