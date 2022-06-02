using FluentValidation;

namespace Riode.WebUI.AppCode.Modules.ProductModule.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateCommand>
    {
        public ProductCreateValidator()
        {
            RuleFor(p=>p.Name).NotEmpty().NotNull();
            RuleFor(p=>p.ShortDescription).NotEmpty().NotNull();
            RuleFor(p=>p.StockKeepingUnit).NotEmpty().NotNull();

            RuleFor(p => p.BrandId).GreaterThan(0).WithMessage("Your entry is not valid.");
            RuleFor(p => p.CategoryId).GreaterThan(0).WithMessage("Your entry is not valid.");

            RuleForEach(p => p.Specifications)
                .ChildRules(cp =>
                {
                    cp.RuleFor(cpi => cpi.Value).NotEmpty().NotNull();
                });
        }
    }
}
