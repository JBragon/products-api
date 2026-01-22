using Products.Application.Common.Caching;
using Products.Application.Common.Exceptions;
using Products.Application.Ports;
using Products.Domain.Entities.Products;

namespace Products.Application.Products.Services.Command
{
    public sealed class InactivateProductHandler : IInactivateProductHandler
    {
        private readonly IProductRepository _repository;
        private readonly IProductCache _cache;

        public InactivateProductHandler(
            IProductRepository repository,
            IProductCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task HandleAsync(InactivateProductCommand command, CancellationToken ct)
        {
            var product = await _repository.GetByIdForUpdateAsync(command.ProductId, ct);

            if (product is null)
                throw new ProductNotFoundException(command.ProductId);

            product.Inactivate();

            await _repository.UpdateAsync(product, ct);

            await _cache.InvalidateAsync(product.Id, ct);
        }
    }
}
