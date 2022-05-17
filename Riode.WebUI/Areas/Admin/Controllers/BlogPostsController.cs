using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.AppCode.Infrastructure;
using Riode.WebUI.AppCode.Modules.BlogPostModule;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using Riode.WebUI.Models.ViewModels;

namespace Riode.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogPostsController : Controller
    {
        private readonly RiodeDbContext db;
        readonly IWebHostEnvironment env;
        readonly IMediator mediator;

        public BlogPostsController(RiodeDbContext db, IWebHostEnvironment env, IMediator mediator)
        {
            this.db=db;
            this.env=env;
            this.mediator=mediator;
        }

        // GET: Admin/BlogPosts
        public async Task<IActionResult> Index(int pageIndex=1, int pageSize=10)
        {
            var query = db.BlogPosts
                .Include(b => b.Category)
                .Where(b => b.DeletedById == null);

            //var riodeDbContext = await query
            //    .Skip((pageIndex-1)*pageSize)
            //    .Take(pageSize)
            //    .ToListAsync();

            //var count = await query.CountAsync();

            //var vModel = new BlogPagedViewModel();
            //vModel.PageIndex = pageIndex;
            //vModel.PageSize = pageSize;
            //vModel.Items = riodeDbContext;
            //vModel.TotalCount = count;

            var pagedModel = new PagedViewModel<BlogPost>(query, pageIndex, pageSize);

            return View(pagedModel);
        }

        // GET: Admin/BlogPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await db.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id && m.DeletedById == null);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // GET: Admin/BlogPosts/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name");
            ViewData["TagId"] = new SelectList(db.PostTags, "Id", "Name");
            return View();
        }

        // POST: Admin/BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPostCreateCommand command)
        {
            var response = await mediator.Send(command);
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name", command.CategoryId);
            ViewData["TagId"] = new SelectList(db.PostTags, "Id", "Name");
            return View(command);
        }

        // GET: Admin/BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await db.BlogPosts
                .Include(bp=>bp.TagCloud)
                .FirstOrDefaultAsync(b=>b.Id == id && b.DeletedById == null);
            if (blogPost == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["TagId"] = new SelectList(db.PostTags, "Id", "Name");
            return View(blogPost);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute]int id, BlogPost blogPost, IFormFile file, int[] tagIds)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }
            if (file == null && string.IsNullOrWhiteSpace(blogPost.ImagePath))
            {
                ModelState.AddModelError("ImagePath", "Please, choose a file!");

            }
            if (ModelState.IsValid)
            {
                try
                {
                    var currentEntity = db.BlogPosts
                        .FirstOrDefault(b => b.Id == id);

                    if (currentEntity == null)
                    {
                        return NotFound();
                    }

                    string oldFileName = currentEntity.ImagePath;

                    if (file != null)
                    {

                        string fileExtension = Path.GetExtension(file.FileName);
                        string name = $"blog-{Guid.NewGuid()}{fileExtension}";
                        string physicalPath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "images", name);
                        using (var fs = new FileStream(physicalPath, FileMode.Create, FileAccess.Write))
                        {
                            file.CopyTo(fs);
                        }
                        currentEntity.ImagePath = name;

                        string oldPhysicalPath = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "images", oldFileName);

                        if (System.IO.File.Exists(oldPhysicalPath))
                        {
                            System.IO.File.Delete(oldPhysicalPath);
                        }
                    }

                    currentEntity.CategoryId = blogPost.CategoryId;
                    currentEntity.Title = blogPost.Title;
                    currentEntity.Body = blogPost.Body;

                    if (tagIds != null && tagIds.Length > 0)
                    {
                        foreach (var item in tagIds)
                        {
                            if (db.BlogPostTagCloud.Any(bptc=>bptc.PostTagId == item && bptc.BlogPostId == blogPost.Id))
                            {
                                continue;
                            }
                            await db.BlogPostTagCloud.AddAsync(new BlogPostTag
                            {
                                BlogPostId = blogPost.Id,
                                PostTagId = item
                            });
                        }
                        await db.SaveChangesAsync();
                    }

                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(db.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["TagId"] = new SelectList(db.PostTags, "Id", "Name");
            return View(blogPost);
        }

        

        // POST: Admin/BlogPosts/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var blogPost = await db.BlogPosts.FirstOrDefaultAsync(bp=>bp.Id== id && bp.DeletedById == null);
            if (blogPost == null)
            {
                return Json(new
                {
                    error = true,
                    message = "The item doesn't exist!"
                });
            }

            blogPost.DeletedById = 1;
            blogPost.DeletedDate = DateTime.UtcNow.AddHours(4);
            await db.SaveChangesAsync();
            return Json(new
            {
                error = false,
                message = "Item deleted!"
            }); ;
        }

        private bool BlogPostExists(int id)
        {
            return db.BlogPosts.Any(e => e.Id == id);
        }
    }
}
