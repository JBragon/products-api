using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Products.Application.Common;
using Products.Application.Common.Caching;
using Products.Application.Ports;
using Products.Application.Products.Dtos;
using Products.Application.Products.Queries;

namespace Products.Application.Products.Services.Query
{
    public sealed class ProductQueryService : IProductQueryService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly IProductCache _cache;

        public ProductQueryService(
            IProductRepository repository,
            IMapper mapper,
            IProductCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ProductDetailDto?> GetDetailAsync(Guid productId, CancellationToken ct)
        {
            var cached = await _cache.GetAsync(productId, ct);
            if (cached is not null)
                return cached;

            var product = await _repository.GetByIdAsync(productId, ct);
            if (product is null)
                return null;

            var dto = _mapper.Map<ProductDetailDto>(product);

            await _cache.SetAsync(productId, dto, ct);

            return dto;
        }

        public async Task<PagedResult<ProductListItemDto>> GetListAsync(ProductListQuery query, CancellationToken ct)
        {
            // limites seguros
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

            // mapeia manualmente (lista é simples e evita AutoMapper overkill)
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
