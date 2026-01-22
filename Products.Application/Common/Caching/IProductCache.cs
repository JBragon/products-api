using Products.Application.Products.Dtos;

namespace Products.Application.Common.Caching
{
    public interface IProductCache
    {
        Task<ProductDetailDto?> GetAsync(Guid productId, CancellationToken ct);
        Task SetAsync(Guid productId, ProductDetailDto dto, CancellationToken ct);
        Task InvalidateAsync(Guid productId, CancellationToken ct);
    }
}
