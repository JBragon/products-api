using Products.Application.Common;
using Products.Application.Products.Dtos;
using Products.Application.Products.Queries;
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
        Task<PagedResult<ProductListItemDto>> GetListAsync(ProductListQuery query, CancellationToken ct);
    }
}
