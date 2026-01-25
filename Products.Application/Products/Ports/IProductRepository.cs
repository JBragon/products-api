using Products.Domain.Entities.Products;

namespace Products.Application.Products.Ports
{
    public interface IProductRepository
    {
        /// <summary>
        /// Gets a product by ID, non-tracking (read-only).
        /// </summary>
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);

        /// <summary>
        /// Searches for products based on various filters and pagination.
        /// </summary>
        /// <param name="q">Text to search in Title, Brand, or Model.</param>
        /// <param name="brand">Exact match for Brand.</param>
        /// <param name="condition">Exact match for Condition.</param>
        /// <param name="page">Page number (1-based).</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>A tuple containing the list of items and the total count of matches.</returns>
        Task<(IReadOnlyCollection<Product> Items, int Total)> SearchAsync(
           string? q,
           string? brand,
           string? condition,
           int page,
           int pageSize,
           CancellationToken ct);

        /// <summary>
        /// Adds a new product to the context.
        /// </summary>
        Task AddAsync(Product product, CancellationToken ct);

        /// <summary>
        /// Persists changes to the database.
        /// </summary>
        Task SaveChangesAsync(CancellationToken ct);

        /// <summary>
        /// Gets a product by ID with tracking enabled, suitable for updates.
        /// </summary>
        Task<Product?> GetByIdForUpdateAsync(Guid id, CancellationToken ct);
    }
}
