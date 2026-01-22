using FluentValidation;
using Products.Api.Contracts;

namespace Products.Api.Validators
{
    public sealed class ProductCreateRequestValidator : AbstractValidator<ProductCreateRequest>
    {
        public ProductCreateRequestValidator()
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

            RuleForEach(x => x.Attributes!)
                .SetValidator(new ProductAttributeRequestValidator())
                .When(x => x.Attributes is not null);

            RuleForEach(x => x.Pictures!)
                .SetValidator(new ProductPictureRequestValidator())
                .When(x => x.Pictures is not null);

            RuleFor(x => x.Pictures)
                .Must(p => p is null || p.Count <= 12)
                .WithMessage("Pictures cannot exceed 12 items.");
        }

        private sealed class ProductAttributeRequestValidator : AbstractValidator<ProductAttributeRequest>
        {
            public ProductAttributeRequestValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(80);

                RuleFor(x => x.Value)
                    .NotEmpty()
                    .MaximumLength(200);
            }
        }

        private sealed class ProductPictureRequestValidator : AbstractValidator<ProductPictureRequest>
        {
            public ProductPictureRequestValidator()
            {
                RuleFor(x => x.Url)
                    .NotEmpty()
                    .MaximumLength(500)
                    .Must(u => Uri.TryCreate(u, UriKind.Absolute, out _))
                    .WithMessage("Picture url must be a valid absolute URL.");
            }
        }
    }
}
