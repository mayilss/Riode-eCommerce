using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.AppCode.Extensions;
using Riode.WebUI.AppCode.Infrastructure;
using Riode.WebUI.Models.DataContexts;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.SubscribeModule
{
    public class SubscribeConfirmCommand : IRequest<CommandJsonResponse>
    {
        public string Token { get; set; }
        public class SubscribeConfirmCommandHandler : IRequestHandler<SubscribeConfirmCommand, CommandJsonResponse>
        {
            readonly RiodeDbContext db;
            readonly IActionContextAccessor ctx;
            public SubscribeConfirmCommandHandler(RiodeDbContext db, IActionContextAccessor ctx)
            {
                this.db = db;
                this.ctx = ctx;
            }
            public async Task<CommandJsonResponse> Handle(SubscribeConfirmCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.Token))
                {
                    ctx.AddModelError("Token", "Token is empty!");
                    goto l1;
                }

                request.Token = request.Token.Decrypt();

                var match = Regex.Match(request.Token, @"(?<id>\d+)-(?<email>.*)");

                if (!match.Success)
                {
                    ctx.AddModelError("Token", "Token is not valid!");
                    goto l1;
                }

                int id = Convert.ToInt32(match.Groups["id"].Value);
                string email = match.Groups["email"].Value;

                var subscribe = await db.Subscribes.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
                if (subscribe == null)
                {
                    ctx.AddModelError("Token", "Subscribtion has not found!");
                    goto l1;
                }
                else if (!email.Equals(subscribe.Email))
                {
                    ctx.AddModelError("Token", "Token is not valid!");
                    goto l1;
                }

                subscribe.AppliedDate = DateTime.UtcNow.AddHours(4);
                await db.SaveChangesAsync(cancellationToken);

                return new CommandJsonResponse(false, "Subscribtion is successful!");

            l1:
                return new CommandJsonResponse(true, ctx.GetError());
            }
        }
    }
}
