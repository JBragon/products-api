using Products.Domain.Common;

namespace Products.Domain.Entities.Products
{
    public sealed class Product : Entity<Guid>
    {
        public string Title { get; private set; } = string.Empty;
        public string Brand { get; private set; } = string.Empty;
        public string Model { get; private set; } = string.Empty;
        public ProductCondition Condition { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; } = true;

        public Money Price { get; private set; } = new Money(0m, "BRL");
        public Installments? Installments { get; private set; }
        public Stock Stock { get; private set; } = new Stock(0);
        public ShippingInfo? Shipping { get; private set; }
        public ReturnsPolicy? Returns { get; private set; }

        public bool PurchaseProtection { get; private set; }
        public Rating? Rating { get; private set; }

        private List<ProductAttribute> _attributes = new();
        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes;

        private List<ProductPicture> _pictures = new();
        public IReadOnlyCollection<ProductPicture> Pictures => _pictures;

        private List<ProductHighlight> _highlights = new();
        public IReadOnlyCollection<ProductHighlight> Highlights => _highlights;

        protected Product() { } // EF

        public Product(
            Guid id,
            string title,
            string brand,
            string model,
            ProductCondition condition,
            Money price,
            Stock stock,
            Installments? installments = null,
            ShippingInfo? shipping = null,
            ReturnsPolicy? returns = null,
            bool purchaseProtection = false,
            Rating? rating = null,
            IEnumerable<ProductAttribute>? attributes = null,
            IEnumerable<ProductPicture>? pictures = null,
            IEnumerable<ProductHighlight>? highlights = null,
            string? description = null
        )
        {
            Id = id;
            Title = title;
            Brand = brand;
            Model = model;
            Condition = condition;

            Price = price;
            Installments = installments;

            Stock = stock;

            Shipping = shipping;
            Returns = returns;

            PurchaseProtection = purchaseProtection;
            Rating = rating;

            Description = description;

            if (attributes is not null) _attributes = attributes.ToList();
            if (pictures is not null) _pictures = pictures.ToList();
            if (highlights is not null) _highlights = highlights.ToList();
        }

        public void Update(
            string title,
            string brand,
            string model,
            ProductCondition condition,
            Money price,
            Stock stock,
            string? description,
            List<ProductAttribute> attributes,
            List<ProductPicture> pictures)
        {
            Title = title;
            Brand = brand;
            Model = model;
            Condition = condition;
            Price = price;
            Stock = stock;
            Description = description;

            _attributes = attributes ?? new();
            _pictures = pictures ?? new();
        }

        public void Inactivate()
        {
            if (!IsActive)
                return;

            IsActive = false;
        }
    }
}
