using MediatR;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.ContactPostModule
{
    public class ContactPostSingleQuery : IRequest<ContactPost>
    {
        public int Id { get; set; }
        public class ContactPostSingleQueryHandler : IRequestHandler<ContactPostSingleQuery, ContactPost>
        {
            readonly RiodeDbContext db;
            public ContactPostSingleQueryHandler(RiodeDbContext db)
            {
                this.db = db;
            }
            public async Task<ContactPost> Handle(ContactPostSingleQuery request, CancellationToken cancellationToken)
            {
                var model = await db.ContactPosts
                    .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
                return model;
            }
        }
    }
}
