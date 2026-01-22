using Products.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Domain.Entities.Products
{
    public sealed class Product : Entity<Guid>
    {
        public string Title { get; private set; } = string.Empty;
        public ProductCondition Condition { get; private set; }
        public string? Description { get; private set; }

        public Money Price { get; private set; } = new Money(0m, "BRL");
        public Stock Stock { get; private set; } = new Stock(0);

        private List<ProductAttribute> _attributes = new();
        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes;

        private List<ProductPicture> _pictures = new();
        public IReadOnlyCollection<ProductPicture> Pictures => _pictures;

        protected Product() { } // EF

        public Product(
           Guid id,
           string title,
           ProductCondition condition,
           Money price,
           Stock stock,
           IEnumerable<ProductAttribute>? attributes = null,
           IEnumerable<ProductPicture>? pictures = null,
           string? description = null
        )
        {
            Id = id;
            Title = title;
            Condition = condition;
            Price = price;
            Stock = stock;
            Description = description;

            if (attributes is not null)
                _attributes = attributes.ToList();

            if (pictures is not null)
                _pictures = pictures.ToList();
        }
    }
}
