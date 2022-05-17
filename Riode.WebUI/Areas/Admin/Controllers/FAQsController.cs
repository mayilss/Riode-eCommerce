using Microsoft.AspNetCore.Mvc;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System.Linq;

namespace Riode.WebUI.Areas.Admin.Controllers
{
    public class FAQsController : Controller
    {
        readonly RiodeDbContext db;
        public FAQsController(RiodeDbContext db)
        {
            this.db = db;
        }
        [Area("Admin")]
        public IActionResult Index()
        {
            var data = db.Faqs.ToList();
            return View(data);
        }

        [Area("Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [Area("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [Area("Admin")]
        public IActionResult Edit(int id)
        {
            var entity = db.Faqs.FirstOrDefault(f => f.Id == id);
            if(entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }
        [Area("Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [Area("Admin")]
        public IActionResult Details(int id)
        {
            var entity = db.Faqs.FirstOrDefault(f=>f.Id == id);
            if(entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }

        [Area("Admin")]
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
