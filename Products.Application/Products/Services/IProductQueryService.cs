using Products.Application.Products.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Products.Services
{
    public interface IProductQueryService
    {
        Task<ProductDetailDto?> GetDetailAsync(Guid productId, CancellationToken ct);
    }
}
