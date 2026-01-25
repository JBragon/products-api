using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Products.Application.Products.Ports;

namespace Products.Infrastructure.Idempotency
{
    /// <summary>
    /// Temporary in-memory storage for idempotency keys.
    /// In a real scenario, this should be replaced by a distributed store like Redis.
    /// </summary>
    public sealed class InMemoryIdempotencyStore : IIdempotencyStore
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<InMemoryIdempotencyStore> _logger;

        public InMemoryIdempotencyStore(
            IMemoryCache cache,
            ILogger<InMemoryIdempotencyStore> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<Guid?> GetAsync(string key, CancellationToken ct)
            => Task.FromResult<Guid?>(_cache.TryGetValue<Guid>(key, out var id) ? id : null);

        public Task StoreAsync(string key, Guid productId, CancellationToken ct)
        {
            _cache.Set(
                key,
                productId,
                TimeSpan.FromMinutes(5)
            );

            _logger.LogDebug("Idempotency key {Key} stored for Product {ProductId}", key, productId);

            return Task.CompletedTask;
        }
    }
}
