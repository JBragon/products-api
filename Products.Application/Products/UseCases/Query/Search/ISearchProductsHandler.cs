using Products.Application.Common.Paging;
using Products.Application.Products.Dtos;

namespace Products.Application.Products.UseCases.Query.Search
{
    public interface ISearchProductsHandler
    {
        Task<PagedResult<ProductListItemDto>> HandleAsync(ProductListQuery query, CancellationToken ct);
    }
}
