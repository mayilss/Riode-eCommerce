using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.ViewModels;
using System.Linq;

namespace Riode.WebUI.Controllers
{
    public class ShopController : Controller
    {
        readonly RiodeDbContext db;
        public ShopController(RiodeDbContext db)
        {
            this.db = db;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = new ShopIndexViewModel();

            model.Brands = db.Brands.ToList();
            model.ProductSizes = db.Sizes.ToList();
            model.ProductColors = db.Colors.ToList();

            model.Categories = db.Categories
                .Include(c=>c.Children)
                .ToList();
            return View(model);
        }
        [AllowAnonymous]
        public IActionResult Details()
        {
            return View();
        }
    }
}
