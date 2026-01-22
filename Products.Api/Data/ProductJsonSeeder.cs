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

            var items = JsonSerializer.Deserialize<List<ProductSeedDto>>(json, options)
                        ?? new();

            foreach (var item in items)
            {
                var product = new Product(
                    id: item.Id,
                    title: item.Title,
                    condition: Enum.Parse<ProductCondition>(item.Condition, true),
                    price: new Money(item.Price.Amount, item.Price.Currency),
                    stock: new Stock(item.Stock.AvailableQuantity),
                    attributes: item.Attributes.Select(a => new ProductAttribute(a.Name, a.Value)),
                    pictures: item.Pictures.Select(p => new ProductPicture(p.Url)),
                    description: item.Description
                );

                db.Products.Add(product);
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
