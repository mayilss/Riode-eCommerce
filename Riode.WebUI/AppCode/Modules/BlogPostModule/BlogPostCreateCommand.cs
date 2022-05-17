using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Riode.WebUI.AppCode.Extensions;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.BlogPostModule
{
    public class BlogPostCreateCommand : IRequest<BlogPost>
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImagePath { get; set; }
        public int CategoryId { get; set; }
        public IFormFile File { get; set; }
        public int[] TagIds { get; set; }

        public class BlogPostCreateCommandHandler : IRequestHandler<BlogPostCreateCommand, BlogPost>
        {
            readonly RiodeDbContext db;
            readonly IWebHostEnvironment env;
            readonly IActionContextAccessor ctx;
            public BlogPostCreateCommandHandler(RiodeDbContext db, IWebHostEnvironment env, IActionContextAccessor ctx)
            {
                this.db = db;
                this.env = env;
                this.ctx = ctx;
            }
            public async Task<BlogPost> Handle(BlogPostCreateCommand request, CancellationToken cancellationToken)
            {
                if (request?.File == null)
                {
                    ctx.AddModelError("ImagePath", "Please, choose a file!");
                }
                if (ctx.ModelIsValid())
                {
                    string fileExtension = Path.GetExtension(request.File.FileName);
                    string name = $"blog-{Guid.NewGuid()}{fileExtension}";
                    string physicalPath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "images", name);
                    using (var fs = new FileStream(physicalPath, FileMode.Create, FileAccess.Write))
                    {
                        await request.File.CopyToAsync(fs, cancellationToken);
                    }

                    var blog = new BlogPost
                    {
                        Title = request.Title,
                        Body = request.Body,
                        CategoryId = request.CategoryId,
                        ImagePath = name
                    };

                    await db.AddAsync(blog, cancellationToken);
                    int affected = await db.SaveChangesAsync(cancellationToken);

                    if (affected > 0 && request.TagIds != null && request.TagIds.Length > 0)
                    {
                        foreach (var item in request.TagIds)
                        {
                            await db.BlogPostTagCloud.AddAsync(new BlogPostTag
                            {
                                BlogPostId = blog.Id,
                                PostTagId = item
                            }, cancellationToken);
                        }
                        await db.SaveChangesAsync(cancellationToken);
                    }
                    return blog;
                }

                return null;
            }
        }
    }
}
