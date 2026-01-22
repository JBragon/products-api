namespace Products.Api.Data.Models
{
    public sealed class ProductSeedDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;

        public string Condition { get; set; } = string.Empty;

        public PriceSeedDto Price { get; set; } = default!;
        public InstallmentsSeedDto? Installments { get; set; }

        public StockSeedDto Stock { get; set; } = default!;

        public ShippingSeedDto? Shipping { get; set; }
        public ReturnsSeedDto? Returns { get; set; }

        public bool PurchaseProtection { get; set; }
        public RatingSeedDto? Rating { get; set; }

        public string? Description { get; set; }

        public List<string> Highlights { get; set; } = new();
        public List<ProductAttributeSeedDto> Attributes { get; set; } = new();
        public List<ProductPictureSeedDto> Pictures { get; set; } = new();
    }

    public sealed class PriceSeedDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BRL";
    }

    public sealed class InstallmentsSeedDto
    {
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public bool InterestFree { get; set; }
    }

    public sealed class StockSeedDto
    {
        public int AvailableQuantity { get; set; }
    }

    public sealed class ShippingSeedDto
    {
        public bool FreeShipping { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
    }

    public sealed class ReturnsSeedDto
    {
        public bool Allowed { get; set; }
        public int WindowDays { get; set; }
    }

    public sealed class RatingSeedDto
    {
        public decimal Average { get; set; }
        public int TotalReviews { get; set; }
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
