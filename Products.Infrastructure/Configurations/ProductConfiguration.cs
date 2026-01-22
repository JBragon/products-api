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
            builder.ToTable("products");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(2000);
            builder.Property(x => x.Condition).IsRequired();

            builder.OwnsOne(x => x.Price, money =>
            {
                money.Property(p => p.Amount).HasColumnName("price_amount").IsRequired();
                money.Property(p => p.Currency).HasColumnName("price_currency").HasMaxLength(5).IsRequired();
            });

            builder.OwnsOne(x => x.Stock, stock =>
            {
                stock.Property(s => s.AvailableQuantity).HasColumnName("available_quantity").IsRequired();
            });

            // ✅ Attributes (backing field)
            builder.Navigation(x => x.Attributes).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany(typeof(ProductAttribute), "_attributes", attrs =>
            {
                attrs.ToTable("product_attributes");
                attrs.WithOwner().HasForeignKey("product_id");

                attrs.Property<Guid>("id");
                attrs.HasKey("id");

                attrs.Property<string>("Name")
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(80);

                attrs.Property<string>("Value")
                    .HasColumnName("value")
                    .IsRequired()
                    .HasMaxLength(200);
            });

            // ✅ Pictures (backing field)
            builder.Navigation(x => x.Pictures).UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany(typeof(ProductPicture), "_pictures", pics =>
            {
                pics.ToTable("product_pictures");
                pics.WithOwner().HasForeignKey("product_id");

                pics.Property<Guid>("id");
                pics.HasKey("id");

                pics.Property<string>("Url")
                    .HasColumnName("url")
                    .IsRequired()
                    .HasMaxLength(500);
            });
        }
    }
}
