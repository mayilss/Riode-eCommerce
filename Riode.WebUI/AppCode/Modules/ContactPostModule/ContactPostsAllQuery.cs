using MediatR;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.ContactPostModule
{
    public class ContactPostsAllQuery : IRequest<IEnumerable<ContactPost>>
    {
        
        public class ContactPostsAllQueryHandler : IRequestHandler<ContactPostsAllQuery, IEnumerable<ContactPost>>
        {
            readonly RiodeDbContext db;
            public ContactPostsAllQueryHandler(RiodeDbContext db)
            {
                this.db = db;
            }
            public async Task<IEnumerable<ContactPost>> Handle(ContactPostsAllQuery request, CancellationToken cancellationToken)
            {
                var data = await db.ContactPosts.ToListAsync(cancellationToken);
                return data;
            }
        }
    }
}
