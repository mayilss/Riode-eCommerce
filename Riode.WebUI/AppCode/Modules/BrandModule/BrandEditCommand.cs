using MediatR;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.BrandModule
{
    public class BrandEditCommand : IRequest<Brand>
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public class BrandEditCommandHandler : IRequestHandler<BrandEditCommand, Brand>
        {
            readonly RiodeDbContext db;
            public BrandEditCommandHandler(RiodeDbContext db)
            {
                this.db = db;
            }
            public async Task<Brand> Handle(BrandEditCommand request, CancellationToken cancellationToken)
            {
                var entity = await db.Brands
                    .FirstOrDefaultAsync(b=>b.Id == request.Id && b.DeletedById == null, cancellationToken);
                if (entity == null)
                {
                    return null;
                }
                entity.Name = request.Name;
                await db.SaveChangesAsync(cancellationToken);

                return entity;
            }
        }
    }
}
