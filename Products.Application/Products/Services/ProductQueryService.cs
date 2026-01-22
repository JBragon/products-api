using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Products.Application.Ports;
using Products.Application.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Products.Services
{
    public sealed class ProductQueryService : IProductQueryService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ProductQueryService(
            IProductRepository repository,
            IMapper mapper,
            IMemoryCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ProductDetailDto?> GetDetailAsync(Guid productId, CancellationToken ct)
        {
            var cacheKey = CacheKeys.ProductDetail(productId);

            if (_cache.TryGetValue(cacheKey, out ProductDetailDto? cached))
                return cached;

            var product = await _repository.GetByIdAsync(productId, ct);
            if (product is null)
                return null;

            var dto = _mapper.Map<ProductDetailDto>(product);

            // TTL curto: bom pra item detail e suficiente pra demonstrar conhecimento
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                SlidingExpiration = TimeSpan.FromSeconds(20)
            };

            _cache.Set(cacheKey, dto, options);

            return dto;
        }
        private static class CacheKeys
        {
            public static string ProductDetail(Guid id) => $"products:detail:{id}";
        }
    }
}
