using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riode.WebUI.AppCode.Modules.ContactPostModule;
using System.Threading.Tasks;

namespace Riode.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactPostsController : Controller
    {
        readonly IMediator mediator;
        public ContactPostsController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [Authorize(Policy = "admin.contactposts.index")]
        public async Task<IActionResult> Index(ContactPostsAllQuery query)
        {
            var data = await mediator.Send(query);
            return View(data);
        }
        [Authorize(Policy = "admin.contactposts.details")]
        public async Task<IActionResult> Details(ContactPostSingleQuery query)
        {
            var data = await mediator.Send(query);
            return View(data);
        }
        [Authorize(Policy = "admin.contactposts.answer")]
        public async Task<IActionResult> Answer(ContactPostSingleQuery query)
        {
            var data = await mediator.Send(query);
            if(data.AnswerDate != null)
			{
                return RedirectToAction(nameof(Details), routeValues: new
				{
                    id = query.Id
				});
			}
            return View(data);
        }
        [HttpPost]
        [Authorize(Policy = "admin.contactposts.answer")]
        public async Task<IActionResult> Answer(ContactPostAnswerCommand command)
        {
            var data = await mediator.Send(command);
			if (!ModelState.IsValid)
			{
                return View(data);
			}

            return RedirectToAction(nameof(Details), routeValues: new
            {
                id = command.Id
            });
        }
    }
}

