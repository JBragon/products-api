using Products.Application.Common;
using Products.Application.Products.Dtos;
using Products.Application.Products.Queries;

namespace Products.Application.Products.Services.Query
{
    public interface IProductQueryService
    {
        Task<ProductDetailDto?> GetDetailAsync(Guid productId, CancellationToken ct);
        Task<PagedResult<ProductListItemDto>> GetListAsync(ProductListQuery query, CancellationToken ct);
    }
}
