using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Riode.WebUI.AppCode.Extensions;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.ProductModule
{
    public class ProductCreateCommand : IRequest<Product>
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string StockKeepingUnit { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public SpecificationKeyValue[] Specifications { get; set; }
        public ProductPricing[] Pricing { get; set; }
        public ImageItem[] Images { get; set; }
        public class ProductCreateCommandHandler : IRequestHandler<ProductCreateCommand, Product>
        {
            readonly RiodeDbContext db;
            readonly IActionContextAccessor ctx;
            readonly IWebHostEnvironment env;
            public ProductCreateCommandHandler(RiodeDbContext db, IActionContextAccessor ctx, IWebHostEnvironment env)
            {
                this.db = db;
                this.ctx = ctx;
                this.env = env;
            }

            public async Task<Product> Handle(ProductCreateCommand request, CancellationToken cancellationToken)
            {
                if (!ctx.ModelIsValid())
                {
                    return null;
                }
                var product = new Product();
                product.Name = request.Name;
                product.StockKeepingUnit = request.StockKeepingUnit;
                product.BrandId = request.BrandId;
                product.CategoryId = request.CategoryId;
                product.ShortDescription = request.ShortDescription;
                product.Description = request.Description;


                if (request.Specifications != null && request.Specifications.Length > 0)
                {
                    product.Specifications = new List<ProductSpecification>();
                    foreach (var spec in request.Specifications)
                    {
                        product.Specifications.Add(new ProductSpecification
                        {
                            SpecificationId = spec.Id,
                            Value = spec.Value
                        });
                    }
                }
                if (request.Images != null && request.Images.Any(i => i.File != null))
                {
                    product.ProductImages = new List<ProductImage>();
                    foreach (var productFile in request.Images.Where(i=> i.File != null))
                    {
                        string name = await env.SaveFile(productFile.File, cancellationToken, "product");

                        product.ProductImages.Add(new ProductImage
                        {
                            ImagePath = name,
                            IsMain = productFile.IsMain
                        });
                    }
                }
                else
                {
                    ctx.AddModelError("Images", "Please, choose image");
                    goto l1;
                }
                if (request.Pricing != null && request.Pricing.Length > 0)
                {
                    product.Pricing = new List<Models.Entities.ProductPricing>();
                    foreach (var pricing in request.Pricing)
                    {
                        product.Pricing.Add(new Models.Entities.ProductPricing
                        {
                            ColorId = pricing.ColorId,
                            SizeId = pricing.SizeId,
                            Price = pricing.Price
                        });
                    }
                }

                await db.Products.AddAsync(product, cancellationToken);

                try
                {
                    await db.SaveChangesAsync(cancellationToken);
                    return product;
                }
                catch (Exception ex)
                {
                    ctx.AddModelError("Name", "There is an error, please try again later.");
                    goto l1;
                }
                
                l1:
                return null;
            }
        }
    }

    public class SpecificationKeyValue
    {
        public int Id { get; set; }
        public string Value { get; set; }
    }
    public class ProductPricing
    {
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int ColorId { get; set; }
        public double Price { get; set; }
    }
    public class ImageItem
    {
        public int? Id { get; set; }
        public bool IsMain { get; set; }
        public string TempPath { get; set; }
        public IFormFile File { get; set; }
    }
}
