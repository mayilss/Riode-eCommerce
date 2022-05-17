using MediatR;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.BrandModule
{
    public class BrandCreateCommand : IRequest<Brand>
    {
        [Required(ErrorMessage = "Can't be left empty!")]
        public string Name { get; set; }

        public class BrandCreateCommandHandler : IRequestHandler<BrandCreateCommand, Brand>
        {
            readonly RiodeDbContext db;
            public BrandCreateCommandHandler(RiodeDbContext db)
            {
                this.db = db;
            }
            public async Task<Brand> Handle(BrandCreateCommand request, CancellationToken cancellationToken)
            {
                var brand = new Brand();
                brand.Name = request.Name;  

                await db.Brands.AddAsync(brand, cancellationToken);
                await db.SaveChangesAsync(cancellationToken);

                return brand;
            }
        }
    }
}
