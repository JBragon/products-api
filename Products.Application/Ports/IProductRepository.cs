using Products.Domain.Entities.Products;

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
        Task SaveChangesAsync(CancellationToken ct);
        Task<Product?> GetByIdForUpdateAsync(Guid id, CancellationToken ct);
    }
}
