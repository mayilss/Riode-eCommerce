using MediatR;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.AppCode.Infrastructure;
using Riode.WebUI.Models.DataContexts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.BrandModule
{
    public class BrandRemoveCommand : IRequest<CommandJsonResponse>
    {
        public int Id { get; set; }
        public class BrandRemoveCommandHandler : IRequestHandler<BrandRemoveCommand, CommandJsonResponse>
        {
            readonly RiodeDbContext db;
            public BrandRemoveCommandHandler(RiodeDbContext db)
            {
                this.db = db;
            }
            public async Task<CommandJsonResponse> Handle(BrandRemoveCommand request, CancellationToken cancellationToken)
            {
                
                var entity = await db.Brands
                    .FirstOrDefaultAsync(b => b.Id == request.Id && b.DeletedById == null, cancellationToken);
                if (entity == null)
                {
                    return new CommandJsonResponse(true, "Item does not exist!");
                }
                entity.DeletedById = 1;
                entity.DeletedDate = DateTime.UtcNow.AddHours(4);
                await db.SaveChangesAsync(cancellationToken);

                return new CommandJsonResponse(false, "Item successfully deleted!");
            }
        }
    }
}
