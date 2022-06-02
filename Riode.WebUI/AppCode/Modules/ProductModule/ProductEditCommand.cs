using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Riode.WebUI.AppCode.Extensions;
using Riode.WebUI.Models.DataContexts;
using Riode.WebUI.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Riode.WebUI.AppCode.Modules.ProductModule
{
    public class ProductEditCommand : IRequest<ProductEditCommandResponse>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string StockKeepingUnit { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public SpecificationKeyValue[] Specifications { get; set; }
        public ProductPricing[] Pricing { get; set; }
        public ImageItem[] Images { get; set; }
        public class ProductEditCommandHandler : IRequestHandler<ProductEditCommand, ProductEditCommandResponse>
        {
            readonly RiodeDbContext db;
            readonly IActionContextAccessor ctx;
            readonly IWebHostEnvironment env;
            readonly IValidator<ProductEditCommand> validtor;

            public ProductEditCommandHandler(
                RiodeDbContext db,
                IActionContextAccessor ctx,
                IWebHostEnvironment env,
                IValidator<ProductEditCommand> validtor)
            {
                this.db = db;
                this.ctx = ctx;
                this.env = env;
                this.validtor = validtor;
            }

            public async Task<ProductEditCommandResponse> Handle(ProductEditCommand request, CancellationToken cancellationToken)
            {
                var result = validtor.Validate(request);
                var response = new ProductEditCommandResponse
                {
                    Product = null,
                    ValidationResult = null
                };


                if (!result.IsValid)
                {
                    response.ValidationResult = result;
                    return response;
                }

                var product = await db.Products.FirstOrDefaultAsync(p=>p.Id == request.Id, cancellationToken);

                if (product == null)
                {
                    response.ValidationResult = result;
                    response.ValidationResult.Errors.Add(new ValidationFailure("Name","Product has not found!"));
                    return response;
                }

                product.Name = request.Name;
                product.StockKeepingUnit = request.StockKeepingUnit;
                product.BrandId = request.BrandId;
                product.CategoryId = request.CategoryId;
                product.ShortDescription = request.ShortDescription;
                product.Description = request.Description;

                if (request.Images != null && request.Images.Any(i => i.File != null))
                {
                    product.ProductImages = new List<ProductImage>();
                    foreach (var productFile in request.Images.Where(i => i.File != null))
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
                    response.Product = product;
                    response.ValidationResult = result;
                    return response;
                }
                catch (Exception ex)
                {
                    response.Product = product;
                    response.ValidationResult = result;
                    response.ValidationResult.Errors
                        .Add(new ValidationFailure("Name", "There is an error, please try again later."));
                    return response;
                }

            l1:
                return null;
            }
        }
    }

    public class ProductEditCommandResponse
    {
        public Product Product { get; set; }
        public ValidationResult ValidationResult { get; set; }
    }


}
