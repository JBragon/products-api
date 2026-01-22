using Microsoft.Extensions.Caching.Memory;
using Products.Application.Products.Ports;

namespace Products.Infrastructure.Idempotency
{
    public sealed class InMemoryIdempotencyStore : IIdempotencyStore
    {
        private readonly IMemoryCache _cache;

        public InMemoryIdempotencyStore(IMemoryCache cache) => _cache = cache;

        public Task<Guid?> GetAsync(string key, CancellationToken ct)
            => Task.FromResult<Guid?>(_cache.TryGetValue<Guid>(key, out var id) ? id : null);

        public Task StoreAsync(string key, Guid productId, CancellationToken ct)
        {
            _cache.Set(
                key,
                productId,
                TimeSpan.FromMinutes(5)
            );

            return Task.CompletedTask;
        }
    }
}
