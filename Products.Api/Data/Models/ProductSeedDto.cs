namespace Products.Api.Data.Models
{
    public sealed class ProductSeedDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public PriceSeedDto Price { get; set; } = default!;
        public StockSeedDto Stock { get; set; } = default!;
        public string? Description { get; set; }
        public List<ProductAttributeSeedDto> Attributes { get; set; } = new();
        public List<ProductPictureSeedDto> Pictures { get; set; } = new();
    }

    public sealed class PriceSeedDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BRL";
    }

    public sealed class StockSeedDto
    {
        public int AvailableQuantity { get; set; }
    }

    public sealed class ProductAttributeSeedDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }

    public sealed class ProductPictureSeedDto
    {
        public string Url { get; set; } = string.Empty;
    }

}
