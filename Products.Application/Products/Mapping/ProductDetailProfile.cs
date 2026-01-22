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
                .ConstructUsing(src => new ProductDetailDto(
                    src.Id,
                    src.Title,
                    src.Brand,
                    src.Model,
                    src.Condition.ToString().ToLowerInvariant(),

                    src.Price.Amount,
                    src.Price.Currency,
                    src.Installments == null ? null : new InstallmentsDto(
                        src.Installments.Quantity,
                        src.Installments.Amount,
                        src.Installments.InterestFree
                    ),

                    src.Stock.AvailableQuantity,

                    src.Pictures.Select(p => p.Url).ToList(),
                    src.Highlights.Select(h => h.Text).ToList(),
                    src.Attributes.Select(a => new ProductAttributeDto(a.Name, a.Value)).ToList(),

                    src.Shipping == null ? null : new ShippingDto(
                        src.Shipping.FreeShipping,
                        src.Shipping.EstimatedDeliveryDate
                    ),
                    src.Returns == null ? null : new ReturnsDto(
                        src.Returns.Allowed,
                        src.Returns.WindowDays
                    ),
                    src.PurchaseProtection,
                    src.Rating == null ? null : new RatingDto(
                        src.Rating.Average,
                        src.Rating.TotalReviews
                    ),

                    src.Description
                ))
                .ForAllMembers(opt => opt.Ignore());
        }
    }

}
