using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.ViewModels;
using System;
using System.Linq;

namespace Riode.WebUI.Controllers
{
    public class BlogsController : Controller
    {
        readonly RiodeDbContext db;
        public BlogsController(RiodeDbContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var data = db.BlogPosts
                .Where(b => b.DeletedById == null)
                .ToList();
            return View(data);
        }
        public IActionResult Details(int id)
        {
            var post = db.BlogPosts
                .Include(bp => bp.TagCloud)
                .ThenInclude(tc => tc.PostTag)
                .FirstOrDefault(b => b.DeletedById == null && b.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            var viewModel = new SinglePostViewModel();
            viewModel.Post = post;

            var tagIdsQuery = post.TagCloud.Select(tc => tc.PostTagId);

            //viewModel.RelatedPosts = db.BlogPosts
            //	.Include(bp=>bp.TagCloud)
            //	.Where(bp => bp.Id != id && bp.DeletedById == null && bp.TagCloud.Any(tc=>Array.IndexOf(tagIds, tc.PostTagId) != -1)).ToList();

            viewModel.RelatedPosts = (from bp in db.BlogPosts
                                      join tc in db.BlogPostTagCloud on bp.Id equals tc.BlogPostId
                                      where bp.Id != id && bp.DeletedById == null && tagIdsQuery.Any(q=>q == tc.PostTagId)
                                      select bp)
                                      .OrderByDescending(bp => bp.Id)
                                      .Take(10)
                                      .Distinct()
                                      .ToList();

            return View(viewModel);
        }
    }
}
