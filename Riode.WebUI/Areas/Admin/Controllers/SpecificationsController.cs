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
    public class SpecificationsController : Controller
    {
        private readonly RiodeDbContext db;

        public SpecificationsController(RiodeDbContext db)
        {
            this.db = db;
        }

        // GET: Admin/Specifications
        [Authorize(Policy = "admin.specifications.index")]
        public async Task<IActionResult> Index()
        {
            return View(await db.Specifications.ToListAsync());
        }

        // GET: Admin/Specifications/Details/5
        [Authorize(Policy = "admin.specifications.details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specification = await db.Specifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specification == null)
            {
                return NotFound();
            }

            return View(specification);
        }

        // GET: Admin/Specifications/Create
        [Authorize(Policy = "admin.specifications.create")]
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.specifications.create")]
        public async Task<IActionResult> Create([Bind("Name,Id")] Specification specification)
        {
            if (ModelState.IsValid)
            {
                db.Add(specification);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specification);
        }

        // GET: Admin/Specifications/Edit/5
        [Authorize(Policy = "admin.specifications.edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specification = await db.Specifications.FindAsync(id);
            if (specification == null)
            {
                return NotFound();
            }
            return View(specification);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.specifications.edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id,CreatedById,CreatedDate,DeletedById,DeletedDate")] Specification specification)
        {
            if (id != specification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(specification);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecificationExists(specification.Id))
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
            return View(specification);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.specifications.delete")]
        public IActionResult Delete([FromRoute] int id)
        {
            var entity = db.Specifications.FirstOrDefault(b => b.Id == id);
            if (entity == null)
            {
                return Json(new
                {
                    error = true,
                    message = "Not Found!"
                });
            }
            db.Specifications.Remove(entity);
            db.SaveChanges();
            return Json(new
            {
                error = false,
                message = "Deleted Successfully!"
            });
        }

        private bool SpecificationExists(int id)
        {
            return db.Specifications.Any(e => e.Id == id);
        }
    }
}
