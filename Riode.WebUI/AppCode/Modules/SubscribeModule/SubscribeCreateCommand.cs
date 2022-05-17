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
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.SubscribeModule
{
    public class SubscribeCreateCommand : IRequest<CommandJsonResponse>
    {
        [Required(ErrorMessage = "Can't be left empty!")]
        [EmailAddress(ErrorMessage = "Email does not fit!")]
        public string Email { get; set; }

        public class SubscribeCreateCommandHandler : IRequestHandler<SubscribeCreateCommand, CommandJsonResponse>
        {
            readonly RiodeDbContext db;
            readonly IConfiguration configuration;
            readonly IActionContextAccessor ctx;
            public SubscribeCreateCommandHandler(RiodeDbContext db, IConfiguration configuration, IActionContextAccessor ctx)
            {
                this.db = db;
                this.configuration = configuration;
                this.ctx = ctx;
            }
            public async Task<CommandJsonResponse> Handle(SubscribeCreateCommand request, CancellationToken cancellationToken)
            {
                var subscribe = await db.Subscribes.FirstOrDefaultAsync(s=>s.Email.Equals(request.Email),cancellationToken);

                if(subscribe == null)
                {
                    subscribe = new Subscribe();
                    subscribe.Email = request.Email;
                    await db.Subscribes.AddAsync(subscribe, cancellationToken);
                    await db.SaveChangesAsync(cancellationToken);
                } 
                else if(subscribe.EmailSent == true)
                {
                    return new CommandJsonResponse
                    {
                        Error = true,
                        Message = "This email address has been used before."
                    };
                }

                string token = $"{subscribe.Id}-{subscribe.Email}".Encrypt();
                string link = $"{ctx.GetAppLink()}/subscribe-confirm?token={token}";

                var emailSuccess = configuration.SendEmail(
                                        subscribe.Email,
                                        "Riode subscription confirmation",
                                        $"Please, <a href=\"{link}\"> click here </a> to confirm your subscribtion.",
                                        cancellationToken);

                if (emailSuccess)
                {
                    subscribe.EmailSent = true;
                    await db.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    return new CommandJsonResponse
                    {
                        Error = true,
                        Message = "Something went wrong. Please, try later."
                    };
                }

                return new CommandJsonResponse
                {
                    Error = false,
                    Message = $"To finish the subscription please, confirm the link in {subscribe.Email}"
                };
            }
        }
    }
}
