using AutoMapper;
using Products.Application.Common.Caching;
using Products.Application.Products.Dtos;
using Products.Application.Products.Ports;

namespace Products.Application.Products.UseCases.Query.GetDetail
{
    public sealed class GetProductDetailHandler : IGetProductDetailHandler
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly IProductCache _cache;

        public GetProductDetailHandler(
            IProductRepository repository,
            IMapper mapper,
            IProductCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ProductDetailDto?> HandleAsync(Guid productId, CancellationToken ct)
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

    }
}
