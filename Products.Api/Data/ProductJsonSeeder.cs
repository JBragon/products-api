using Microsoft.EntityFrameworkCore;
using Products.Api.Data.Models;
using Products.Domain.Entities.Products;
using Products.Infrastructure;
using System.Text.Json;

namespace Products.Api.Data
{
    public static class ProductJsonSeeder
    {
        public static async Task SeedAsync(
            DatabaseContext db,
            IWebHostEnvironment env,
            CancellationToken ct = default)
        {
            if (await db.Products.AnyAsync(ct))
                return;

            var filePath = Path.Combine(env.ContentRootPath, "Data", "products.json");
            if (!File.Exists(filePath))
                return;

            var json = await File.ReadAllTextAsync(filePath, ct);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var items = JsonSerializer.Deserialize<List<ProductSeedDto>>(json, options) ?? new();

            foreach (var item in items)
            {
                var installments = item.Installments is null
                    ? null
                    : new Installments(item.Installments.Quantity, item.Installments.Amount, item.Installments.InterestFree);

                var shipping = item.Shipping is null
                    ? null
                    : new ShippingInfo(item.Shipping.FreeShipping, item.Shipping.EstimatedDeliveryDate);

                var returns = item.Returns is null
                    ? null
                    : new ReturnsPolicy(item.Returns.Allowed, item.Returns.WindowDays);

                var rating = item.Rating is null
                    ? null
                    : new Rating(item.Rating.Average, item.Rating.TotalReviews);

                var product = new Product(
                    id: item.Id,
                    title: item.Title,
                    brand: item.Brand,
                    model: item.Model,
                    condition: Enum.Parse<ProductCondition>(item.Condition, true),
                    price: new Money(item.Price.Amount, item.Price.Currency),
                    stock: new Stock(item.Stock.AvailableQuantity),
                    installments: installments,
                    shipping: shipping,
                    returns: returns,
                    purchaseProtection: item.PurchaseProtection,
                    rating: rating,
                    attributes: item.Attributes.Select(a => new ProductAttribute(a.Name, a.Value)),
                    pictures: item.Pictures.Select(p => new ProductPicture(p.Url)),
                    highlights: item.Highlights.Select(h => new ProductHighlight(h)),
                    description: item.Description
                );

                db.Products.Add(product);
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
