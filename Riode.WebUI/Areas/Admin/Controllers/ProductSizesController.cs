using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;

namespace Riode.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductSizesController : Controller
    {
        private readonly RiodeDbContext db;

        public ProductSizesController(RiodeDbContext db)
        {
            this.db = db;
        }

        // GET: Admin/ProductSizes
        [Authorize(Policy = "admin.productsizes.index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.Sizes.ToListAsync());
        }

        // GET: Admin/ProductSizes/Details/5
        [Authorize(Policy = "admin.productsizes.details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSize = await db.Sizes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productSize == null)
            {
                return NotFound();
            }

            return View(productSize);
        }

        // GET: Admin/ProductSizes/Create
        [Authorize(Policy = "admin.productsizes.create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.productsizes.create")]
        public async Task<IActionResult> Create([Bind("Id,ShortName,Name")] ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                db.Add(productSize);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productSize);
        }

        // GET: Admin/ProductSizes/Edit/5
        [Authorize(Policy = "admin.productsizes.edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSize = await db.Sizes.FindAsync(id);
            if (productSize == null)
            {
                return NotFound();
            }
            return View(productSize);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.productsizes.edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ShortName,Name")] ProductSize productSize)
        {
            if (id != productSize.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(productSize);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductSizeExists(productSize.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productSize);
        }

        // GET: Admin/ProductSizes/Delete/5
        [Authorize(Policy = "admin.productsizes.delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSize = await db.Sizes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productSize == null)
            {
                return NotFound();
            }

            return View(productSize);
        }

        // POST: Admin/ProductSizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productSize = await db.Sizes.FindAsync(id);
            db.Sizes.Remove(productSize);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductSizeExists(int id)
        {
            return db.Sizes.Any(e => e.Id == id);
        }
    }
}
