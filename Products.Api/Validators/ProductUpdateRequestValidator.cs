using FluentValidation;
using Products.Api.Contracts;

namespace Products.Api.Validators
{
    public sealed class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
    {
        public ProductUpdateRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.Brand)
                .NotEmpty()
                .Length(2, 80);

            RuleFor(x => x.Model)
                .NotEmpty()
                .Length(1, 120);

            RuleFor(x => x.Condition)
                .NotEmpty()
                .Must(c => c.Equals("new", StringComparison.OrdinalIgnoreCase)
                       || c.Equals("used", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Condition must be 'new' or 'used'.");

            RuleFor(x => x.Price)
                .GreaterThan(0);

            RuleFor(x => x.Currency)
                .NotEmpty()
                .Length(3, 5);

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Description)
                .MaximumLength(5000);
        }
    }
}
