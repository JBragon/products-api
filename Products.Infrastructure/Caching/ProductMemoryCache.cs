using Microsoft.Extensions.Caching.Memory;
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

        public ProductMemoryCache(IMemoryCache cache)
            => _cache = cache;

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
            return Task.CompletedTask;
        }

        public Task InvalidateAsync(Guid productId, CancellationToken ct)
        {
            _cache.Remove(GetKey(productId));
            return Task.CompletedTask;
        }

        public static string GetKey(Guid productId)
            => $"products:detail:{productId}";
    }
}
