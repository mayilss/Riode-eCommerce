using MediatR;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.BrandModule
{
    public class BrandsAllQuery : IRequest<IEnumerable<Brand>>
    {
        public class BrandsAllQueryHandler : IRequestHandler<BrandsAllQuery, IEnumerable<Brand>>
        {
            readonly RiodeDbContext db;
            public BrandsAllQueryHandler(RiodeDbContext db)
            {
                this.db = db;
            }
            public async Task<IEnumerable<Brand>> Handle(BrandsAllQuery request, CancellationToken cancellationToken)
            {
                var model = await db.Brands.Where(b => b.DeletedById == null).ToListAsync(cancellationToken);
                return model;
            }
        }
    }
}
