using Products.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Ports
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<(IReadOnlyCollection<Product> Items, int Total)> SearchAsync(
           string? q,
           string? brand,
           string? condition,
           int page,
           int pageSize,
           CancellationToken ct);
        Task AddAsync(Product product, CancellationToken ct);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct);
    }
}
