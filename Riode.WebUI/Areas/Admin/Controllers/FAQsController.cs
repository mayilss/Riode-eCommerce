using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System.Linq;

namespace Riode.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FAQsController : Controller
    {
        readonly RiodeDbContext db;
        public FAQsController(RiodeDbContext db)
        {
            this.db = db;
        }
        [Authorize(Policy = "admin.faqs.index")]
        public IActionResult Index()
        {
            var data = db.Faqs.ToList();
            return View(data);
        }

        [Authorize(Policy = "admin.faqs.create")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.faqs.create")]
        public IActionResult Create(Faq model)
        {
            if (ModelState.IsValid)
            {
                db.Faqs.Add(model);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Policy = "admin.faqs.edit")]
        public IActionResult Edit(int id)
        {
            var entity = db.Faqs.FirstOrDefault(f => f.Id == id);
            if(entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "admin.faqs.edit")]
        public IActionResult Edit([FromRoute]int id, Faq model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {

                db.Faqs.Update(model);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [Authorize(Policy = "admin.faqs.details")]
        public IActionResult Details(int id)
        {
            var entity = db.Faqs.FirstOrDefault(f=>f.Id == id);
            if(entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }

        [Authorize(Policy = "admin.faqs.delete")]
        public IActionResult Delete(int id)
        {
            var entity = db.Faqs.FirstOrDefault(f => f.Id == id);
            if (entity == null)
            {
                return Json(new
                {
                    error = true,
                    message = "Not Found"
                });
            }
            db.Faqs.Remove(entity);
            db.SaveChanges();
            return Json(new
            {
                error = false,
                message = "Deleted Successfully!"
            });
        }
    }
}
