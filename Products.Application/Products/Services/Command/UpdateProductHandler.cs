using Products.Application.Common.Caching;
using Products.Application.Common.Exceptions;
using Products.Application.Ports;
using Products.Domain.Entities.Products;

namespace Products.Application.Products.Services.Command
{
    public sealed class UpdateProductHandler : IUpdateProductHandler
    {
        private readonly IProductRepository _repository;
        private readonly IProductCache _cache;

        public UpdateProductHandler(IProductRepository repository, IProductCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task HandleAsync(UpdateProductCommand command, CancellationToken ct)
        {
            var product = await _repository.GetByIdForUpdateAsync(command.ProductId, ct);

            if (product is null)
                throw new ProductNotFoundException(command.ProductId);

            var condition = Enum.Parse<ProductCondition>(command.Condition, true);

            product.Update(
                title: command.Title,
                brand: command.Brand,
                model: command.Model,
                condition: condition,
                price: new Money(command.Price, command.Currency),
                stock: new Stock(command.Stock),
                description: command.Description,
                attributes: command.Attributes
                    .Select(a => new ProductAttribute(a.Name, a.Value))
                    .ToList(),
                pictures: command.Pictures
                    .Select(p => new ProductPicture(p))
                    .ToList()
            );

            await _repository.UpdateAsync(product, ct);
            await _cache.InvalidateAsync(product.Id, ct);
        }
    }
}
