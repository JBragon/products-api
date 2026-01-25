using AutoMapper;
using Microsoft.Extensions.Logging;
using Products.Application.Common.Caching;
using Products.Application.Products.Dtos;
using Products.Application.Products.Ports;

namespace Products.Application.Products.UseCases.Query.GetDetail
{
    /// <summary>
    /// Retrieves full product details.
    /// Utilizes a "look-aside" cache strategy to minimize database hits.
    /// </summary>
    public sealed class GetProductDetailHandler : IGetProductDetailHandler
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly IProductCache _cache;
        private readonly ILogger<GetProductDetailHandler> _logger;

        public GetProductDetailHandler(
            IProductRepository repository,
            IMapper mapper,
            IProductCache cache,
            ILogger<GetProductDetailHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<ProductDetailDto?> HandleAsync(Guid productId, CancellationToken ct)
        {
            var cached = await _cache.GetAsync(productId, ct);
            if (cached is not null)
            {
                _logger.LogDebug("Cache HIT for product {ProductId}", productId);
                return cached;
            }

            _logger.LogDebug("Cache MISS for product {ProductId}. Fetching from DB...", productId);
            var product = await _repository.GetByIdAsync(productId, ct);
            if (product is null)
                return null;

            var dto = _mapper.Map<ProductDetailDto>(product);

            await _cache.SetAsync(productId, dto, ct);

            return dto;
        }

    }
}
