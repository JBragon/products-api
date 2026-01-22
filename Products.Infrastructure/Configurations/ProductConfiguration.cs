using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Products.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Infrastructure.Configurations
{
    public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.Condition)
                .IsRequired();

            builder.OwnsOne(x => x.Price, money =>
            {
                money.Property(p => p.Amount)
                    .IsRequired();

                money.Property(p => p.Currency)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            builder.OwnsOne(x => x.Stock, stock =>
            {
                stock.Property(s => s.AvailableQuantity)
                    .IsRequired();
            });

            builder.Ignore(x => x.Attributes);
            builder.Ignore(x => x.Pictures);

            builder.OwnsMany<ProductAttribute>("_attributes", attrs =>
            {
                attrs.WithOwner().HasForeignKey("product_id");

                attrs.Property<Guid>("id");
                attrs.HasKey("id");

                attrs.Property(a => a.Name)
                    .IsRequired()
                    .HasMaxLength(80);

                attrs.Property(a => a.Value)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            builder.OwnsMany<ProductPicture>("_pictures", pics =>
            {
                pics.WithOwner().HasForeignKey("product_id");

                pics.Property<Guid>("id");
                pics.HasKey("id");

                pics.Property(p => p.Url)
                    .IsRequired()
                    .HasMaxLength(500);
            });
        }
    }
}
