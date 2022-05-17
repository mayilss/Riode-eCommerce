using MediatR;
using Microsoft.AspNetCore.Mvc;
using Riode.WebUI.AppCode.Modules.BrandModule;
using Riode.WebUI.AppCode.Modules.SubscribeModule;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Riode.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandsController : Controller
    {
        readonly IMediator mediator;
        public BrandsController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        public async Task<IActionResult> Index()
        {
            var a = await mediator.Send(new SubscribeCreateCommand
            {
                Email = "mayilss@code.edu.az"
            });
            var data = await mediator.Send(new BrandsAllQuery());
            return View(data);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandCreateCommand command)
        {
            if (ModelState.IsValid)
            {
                var response = await mediator.Send(command);
                return RedirectToAction(nameof(Index));
            }
            return View(command);
        }
        public async Task<IActionResult> Edit(BrandSingleQuery query)
        {
            var entity = await mediator.Send(query);
            if(entity == null)
            {
                return NotFound();
            }
            var command = new BrandEditCommand();
            command.Id = entity.Id; 
            command.Name = entity.Name;
            return View(command);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit ([FromRoute]int id, BrandEditCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                var response = await mediator.Send(command);
                return RedirectToAction(nameof(Index));
            }
            return View(command);
        }
        public async Task<IActionResult> Details(BrandSingleQuery query)
        {
            var entity = await mediator.Send(query);
            if (entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }
        [HttpPost]
        public async Task<IActionResult> Delete (BrandRemoveCommand command)
        {
            var response = await mediator.Send(command);
            return Json(response);
        } 
    }
}
