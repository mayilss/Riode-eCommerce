using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System.Drawing;
using System.Linq;

namespace Riode.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductColorsController : Controller
    {
        readonly RiodeDbContext db;
        public ProductColorsController(RiodeDbContext db)
        {
            this.db = db;
        }

        [Authorize(Policy = "admin.productcolors.index")]
        public IActionResult Index()
        {
            var data = db.Colors.ToList();
            return View(data);
        }

        [Authorize(Policy = "admin.productcolors.create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.productcolors.create")]
        public IActionResult Create(ProductColor model)
        {
            if (ModelState.IsValid)
            {
                db.Colors.Add(model);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Policy = "admin.productcolors.edit")]
        public IActionResult Edit([FromRoute] int id)
        {
            var entity = db.Colors.FirstOrDefault(c => c.Id == id);
            if (entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.productcolors.edit")]
        public IActionResult Edit([FromRoute] int id, ProductColor model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                db.Colors.Update(model);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Policy = "admin.productcolors.details")]
        public IActionResult Details([FromRoute] int id)
        {
            var entity = db.Colors.FirstOrDefault(c => c.Id == id);
            if (entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }

        [Authorize(Policy = "admin.productcolors.delete")]
        public IActionResult Delete([FromRoute] int id)
        {
            var entity = db.Colors.FirstOrDefault(c => c.Id == id);
            if (entity == null)
            {
                return Json(new
                {
                    error = true,
                    message = "Not Found!"
                }); ;
            }
            db.Colors.Remove(entity);
            db.SaveChanges();
            return Json(new
            {
                error = false,
                message = "Deleted Successfully!"
            });
        }
    }
}
