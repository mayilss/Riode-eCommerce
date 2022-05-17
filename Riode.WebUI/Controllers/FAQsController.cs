using Microsoft.AspNetCore.Mvc;
using Riode.WebUI.Models.DataContexts;

namespace Riode.WebUI.Controllers
{
    public class FAQsController : Controller
    {
        readonly RiodeDbContext db;
        public FAQsController(RiodeDbContext db)
		{
            this.db = db;
		}
        public IActionResult Index()
        {
            var faqs = db.Faqs;
            return View(faqs);
        }
    }
}
