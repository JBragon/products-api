using Microsoft.Extensions.Logging;
using Products.Application.Common.Caching;
using Products.Application.Common.Exceptions;
using Products.Application.Products.Ports;

namespace Products.Application.Products.UseCases.Delete
{
    /// <summary>
    /// Handles the logical deletion (inactivation) of a product.
    /// Marks the product as inactive and invalidates the cache.
    /// </summary>
    public sealed class InactivateProductHandler : IInactivateProductHandler
    {
        private readonly IProductRepository _repository;
        private readonly IProductCache _cache;
        private readonly ILogger<InactivateProductHandler> _logger;

        public InactivateProductHandler(
            IProductRepository repository,
            IProductCache cache,
            ILogger<InactivateProductHandler> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task HandleAsync(InactivateProductCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Inactivating product {ProductId}...", command.ProductId);
            var product = await _repository.GetByIdForUpdateAsync(command.ProductId, ct);

            if (product is null)
                throw new ProductNotFoundException(command.ProductId);

            product.Inactivate();

            await _repository.SaveChangesAsync(ct);

            await _cache.InvalidateAsync(product.Id, ct);
            _logger.LogInformation("Product {ProductId} inactivated successfully.", command.ProductId);
        }
    }
}
