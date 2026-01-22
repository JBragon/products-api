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
            CreateMap<ProductAttribute, ProductAttributeDto>();

            CreateMap<Product, ProductDetailDto>()
                // força o AutoMapper a criar o DTO pelo construtor
                .ConstructUsing(src => new ProductDetailDto(
                    src.Id,
                    src.Title,
                    src.Condition.ToString().ToLowerInvariant(),
                    src.Price.Amount,
                    src.Price.Currency,
                    src.Stock.AvailableQuantity,
                    src.Pictures.Select(p => p.Url).ToList(),
                    src.Attributes.Select(a => new ProductAttributeDto(a.Name, a.Value)).ToList(),
                    src.Description
                ))
                // e impede ele de tentar mapear membros depois
                .ForAllMembers(opt => opt.Ignore());
        }
    }
}
