using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Products.Application.Common.Caching;
using Products.Application.Products.Dtos;

namespace Products.Infrastructure.Caching
{
    /// <summary>
    /// In-memory implementation of product cache using IMemoryCache.
    /// Configures absolute and sliding expiration for entries.
    /// </summary>
    public sealed class ProductMemoryCache : IProductCache
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<ProductMemoryCache> _logger;

        public ProductMemoryCache(
            IMemoryCache cache,
            ILogger<ProductMemoryCache> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<ProductDetailDto?> GetAsync(Guid productId, CancellationToken ct)
        {
            _cache.TryGetValue(GetKey(productId), out ProductDetailDto? dto);
            return Task.FromResult(dto);
        }

        public Task SetAsync(Guid productId, ProductDetailDto dto, CancellationToken ct)
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                SlidingExpiration = TimeSpan.FromSeconds(20)
            };

            _cache.Set(GetKey(productId), dto, options);
            _logger.LogDebug("Cache SET for {ProductId}", productId);
            return Task.CompletedTask;
        }

        public Task InvalidateAsync(Guid productId, CancellationToken ct)
        {
            _cache.Remove(GetKey(productId));
            _logger.LogDebug("Cache REMOVED for {ProductId}", productId);
            return Task.CompletedTask;
        }

        public static string GetKey(Guid productId)
            => $"products:detail:{productId}";
    }
}
