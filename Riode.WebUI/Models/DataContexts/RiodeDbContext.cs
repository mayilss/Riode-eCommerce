using Microsoft.EntityFrameworkCore;
using Riode.WebUI.Models.Entities;

namespace Riode.WebUI.Models.DataContexts
{
    public class RiodeDbContext : DbContext
    {
        public RiodeDbContext(DbContextOptions options)
            : base(options)
        {

        }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<Specification> Specifications { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<BlogPostTag> BlogPostTagCloud { get; set; }
        public DbSet<ContactPost> ContactPosts { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSize> Sizes { get; set; }
        public DbSet<ProductColor> Colors { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications{ get; set; }
        public DbSet<ProductPricing> ProductPricing { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BlogPostTag>(e =>
            {
                e.HasKey(k=>new { k.BlogPostId, k.PostTagId });
            });
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductSpecification>(e =>
            {
                e.HasKey(k => new { k.ProductId, k.SpecificationId });
            });
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ProductPricing>(e =>
            {
                e.HasKey(k => new { k.ProductId, k.SizeId, k.ColorId });
            });
        }
    }
}
