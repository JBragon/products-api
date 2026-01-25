using Products.Application.Products.Ports;
using Products.Domain.Entities.Products;

namespace Products.Application.Products.UseCases.Create
{
    /// <summary>
    /// Handles the creation of new products.
    /// Ensures idempotency using the provided key to prevent duplicate records.
    /// </summary>
    public sealed class CreateProductHandler: ICreateProductHandler
    {
        private readonly IProductRepository _repository;
        private readonly IIdempotencyStore _idempotency;

        public CreateProductHandler(
            IProductRepository repository,
            IIdempotencyStore idempotency)
        {
            _repository = repository;
            _idempotency = idempotency;
        }

        public async Task<Guid> HandleAsync(
            CreateProductCommand command,
            string idempotencyKey,
            CancellationToken ct)
        {
            var existing = await _idempotency.GetAsync(idempotencyKey, ct);
            if (existing.HasValue)
                return existing.Value;

            var condition = Enum.Parse<ProductCondition>(command.Condition, true);

            var product = new Product(
                id: command.ProductId,
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

            await _repository.AddAsync(product, ct);
            await _idempotency.StoreAsync(idempotencyKey, product.Id, ct);

            return product.Id;
        }
    }
}
