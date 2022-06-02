using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.AppCode.Extensions;
using Riode.WebUI.AppCode.Infrastructure;
using Riode.WebUI.AppCode.Modules.ProductModule;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;

namespace Riode.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly RiodeDbContext _context;
        private readonly IMediator mediator;

        public ProductsController(RiodeDbContext context, IMediator mediator)
        {
            _context = context;
            this.mediator = mediator;   
        }

        // GET: Admin/Products
        [Authorize(Policy = "admin.products.index")]
        public async Task<IActionResult> Index()
        {
            var riodeDbContext = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages.Where(i=>i.IsMain == true));
            return View(await riodeDbContext.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        [Authorize(Policy = "admin.products.details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        [Authorize(Policy = "admin.products.create")]
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["Colors"] = new SelectList(_context.Colors, "Id", "Name");
            ViewData["Sizes"] = new SelectList(_context.Sizes, "Id", "Name");
            ViewData["Specifications"] = _context.Specifications.Where(s=> s.DeletedById == null).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.products.create")]
        public async Task<IActionResult> Create(ProductCreateCommand model)
        {
            var response = await mediator.Send(model);

            if (response?.ValidationResult != null && !response.ValidationResult.IsValid)
            {
                return Json(response.ValidationResult);
            }
            return Json(new CommandJsonResponse(false, $"Succesful! New products ID is: {response.Product.Id}"));

        }

        // GET: Admin/Products/Edit/5
        [Authorize(Policy = "admin.products.edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p=>p.ProductImages)
                .Include(p=>p.Specifications)
                .Include(p=>p.Pricing).ThenInclude(c=>c.Color)
                .Include(p=>p.Pricing).ThenInclude(s=>s.Size)
                .FirstOrDefaultAsync(p=>p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["Colors"] = new SelectList(_context.Colors, "Id", "Name");
            ViewData["Sizes"] = new SelectList(_context.Sizes, "Id", "Name");
            ViewData["Specifications"] = _context.Specifications.Where(s => s.DeletedById == null).ToList();
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.products.edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Name,StockKeepingUnit,BrandId,CategoryId,ShortDescription,Description,Id,CreatedById,CreatedDate,DeletedById,DeletedDate")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        [Authorize(Policy = "admin.products.delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
