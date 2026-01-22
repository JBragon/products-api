using AutoMapper;
using Products.Application.Products.Dtos;
using Products.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Products.Mapping
{
    public sealed class ProductDetailProfile : Profile
    {
        public ProductDetailProfile()
        {
            CreateMap<Product, ProductDetailDto>()
                .ForMember(d => d.Condition,
                    o => o.MapFrom(s => s.Condition.ToString().ToLowerInvariant()))
                .ForMember(d => d.Price,
                    o => o.MapFrom(s => s.Price.Amount))
                .ForMember(d => d.Currency,
                    o => o.MapFrom(s => s.Price.Currency))
                .ForMember(d => d.AvailableQuantity,
                    o => o.MapFrom(s => s.Stock.AvailableQuantity))
                .ForMember(d => d.Pictures,
                    o => o.MapFrom(s => s.Pictures.Select(p => p.Url)));

            CreateMap<ProductAttribute, ProductAttributeDto>();
        }
    }
}
