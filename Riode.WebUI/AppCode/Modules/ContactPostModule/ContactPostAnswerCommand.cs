using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Riode.WebUI.AppCode.Extensions;
using Riode.WebUI.AppCode.Infrastructure;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.ContactPostModule
{
	public class ContactPostAnswerCommand : IRequest<ContactPost>
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Can't be left empty")]
		[MinLength(3, ErrorMessage = "At least 3 characters must be used!")]
		public string AnswerMessage { get; set; }
		public class ContactPostAnswerCommandHandler : IRequestHandler<ContactPostAnswerCommand, ContactPost>
		{
			readonly RiodeDbContext db;
			readonly IActionContextAccessor ctx;
			readonly IConfiguration configuration;
			public ContactPostAnswerCommandHandler(RiodeDbContext db, IActionContextAccessor ctx, IConfiguration configuration)
			{
				this.db = db;
				this.ctx = ctx;
				this.configuration = configuration;
			}

			public async Task<ContactPost> Handle(ContactPostAnswerCommand request, CancellationToken cancellationToken)
			{
				l1:
				if (!ctx.ModelIsValid())
				{
					return new ContactPost
					{
						Id = request.Id,
						AnswerMessage = request.AnswerMessage
					};
				}
				var post = await db.ContactPosts.FirstOrDefaultAsync(cp => cp.Id == request.Id, cancellationToken);
				if (post == null)
				{
					ctx.AddModelError("AnswerMessage", "Not Found");
					goto l1;
				}
				else if(post.AnswerDate != null)
				{
					ctx.AddModelError("AnswerMessage", "Already answered");
					goto l1;
				}
				post.AnswerDate = DateTime.UtcNow.AddHours(4);
				post.AnswerMessage = request.AnswerMessage;

				StringBuilder sb = new StringBuilder();

				sb.Append("<ul>");
				sb.Append($"<li>{post.Message}</li>");
				sb.Append($"<li>{request.AnswerMessage}</li>");
				sb.Append("</ul>");

				var emailSuccess = configuration.SendEmail(
										post.Email,
										"Riode Support Center",
										sb.ToString(),
										cancellationToken);

				if (emailSuccess)
				{
					await db.SaveChangesAsync(cancellationToken);
				}
				else
				{
					ctx.AddModelError("Message", "Something went wrong, please try again later.");
				}


				return post;
			}
		}
	}
}
