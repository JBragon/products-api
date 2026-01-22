using System.ComponentModel.DataAnnotations;

namespace Products.Api.Contracts
{
    public sealed class ProductCreateRequest
    {
        [Required]
        public string Title { get; init; } = string.Empty;

        [Required]
        public string Brand { get; init; } = string.Empty;

        [Required]
        public string Model { get; init; } = string.Empty;

        [Required]
        public string Condition { get; init; } = string.Empty; // new / used

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; init; }

        [Required]
        public string Currency { get; init; } = "BRL";

        [Range(0, int.MaxValue)]
        public int Stock { get; init; }

        public string? Description { get; init; }

        public List<ProductAttributeRequest>? Attributes { get; init; }
        public List<ProductPictureRequest>? Pictures { get; init; }
    }

    public sealed class ProductAttributeRequest
    {
        public string Name { get; init; } = string.Empty;
        public string Value { get; init; } = string.Empty;
    }

    public sealed class ProductPictureRequest
    {
        public string Url { get; init; } = string.Empty;
    }
}
