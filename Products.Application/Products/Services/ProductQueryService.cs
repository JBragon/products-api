using AutoMapper;
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

        public ProductQueryService(
            IProductRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ProductDetailDto?> GetDetailAsync(
            Guid productId,
            CancellationToken ct)
        {
            var product = await _repository.GetByIdAsync(productId, ct);
            return product is null ? null : _mapper.Map<ProductDetailDto>(product);
        }
    }
}
