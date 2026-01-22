using Products.Application.Common.Paging;
using Products.Application.Products.Dtos;
using Products.Application.Products.Ports;

namespace Products.Application.Products.UseCases.Query.Search
{
    public sealed class SearchProductsHandler : ISearchProductsHandler
    {
        private readonly IProductRepository _repository;

        public SearchProductsHandler(IProductRepository repository)
            => _repository = repository;

        public async Task<PagedResult<ProductListItemDto>> HandleAsync(ProductListQuery query, CancellationToken ct)
        {
            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize is < 1 ? 10 : query.PageSize;
            if (pageSize > 50) pageSize = 50;

            var (items, total) = await _repository.SearchAsync(
                q: query.Q,
                brand: query.Brand,
                condition: query.Condition,
                page: page,
                pageSize: pageSize,
                ct: ct
            );

            var dtos = items.Select(p => new ProductListItemDto(
                p.Id,
                p.Title,
                p.Brand,
                p.Model,
                p.Condition.ToString().ToLowerInvariant(),
                p.Price.Amount,
                p.Price.Currency,
                p.Pictures.FirstOrDefault()?.Url,
                p.Stock.AvailableQuantity,
                p.Rating?.Average,
                p.Rating?.TotalReviews
            )).ToList();

            return new PagedResult<ProductListItemDto>(dtos, page, pageSize, total);
        }
    }
}
