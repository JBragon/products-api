using Microsoft.Extensions.Logging;
using Products.Application.Common.Paging;
using Products.Application.Products.Dtos;
using Products.Application.Products.Ports;
using Products.Application.Products.Queries;

namespace Products.Application.Products.UseCases.Query.Search
{
    /// <summary>
    /// Handles product listing with support for search, filtering, and pagination.
    /// Queries the repository directly (no cache) to ensure fresh search results.
    /// </summary>
    public sealed class SearchProductsHandler : ISearchProductsHandler
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<SearchProductsHandler> _logger;

        public SearchProductsHandler(
            IProductRepository repository,
            ILogger<SearchProductsHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedResult<ProductListItemDto>> HandleAsync(ProductListQuery query, CancellationToken ct)
        {
            var page = query.Page < 1 ? 1 : query.Page;
            var pageSize = query.PageSize is < 1 ? 10 : query.PageSize;
            if (pageSize > 50) pageSize = 50;

            _logger.LogDebug("Searching products. Page: {Page}, Size: {Size}, Query: '{Q}', Brand: '{Brand}'", 
                page, pageSize, query.Q, query.Brand);

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
