using Products.Application.Products.Dtos;

namespace Products.Application.Common.Caching
{
    /// <summary>
    /// Contract for product caching operations (Read specific).
    /// </summary>
    public interface IProductCache
    {
        /// <summary>
        /// Tries to retrieve a product detail DTO from cache.
        /// </summary>
        Task<ProductDetailDto?> GetAsync(Guid productId, CancellationToken ct);
        
        /// <summary>
        /// Stores a product detail DTO in cache with defined expiration policies.
        /// </summary>
        Task SetAsync(Guid productId, ProductDetailDto dto, CancellationToken ct);
        
        /// <summary>
        /// Removes a product from the cache (invalidation).
        /// </summary>
        Task InvalidateAsync(Guid productId, CancellationToken ct);
    }
}
