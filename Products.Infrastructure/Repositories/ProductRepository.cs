using Microsoft.EntityFrameworkCore;
using Products.Application.Ports;
using Products.Domain.Entities.Products;

namespace Products.Infrastructure.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _db;

        public ProductRepository(DatabaseContext db) => _db = db;

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct) =>
            _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<Product?> GetByIdForUpdateAsync(Guid id, CancellationToken ct) =>
            _db.Products
                .AsTracking()
                .Include("_attributes")
                .Include("_pictures")
                .FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<(IReadOnlyCollection<Product> Items, int Total)> SearchAsync
        (
            string? q,
            string? brand,
            string? condition,
            int page,
            int pageSize,
            CancellationToken ct
        )
        {
            var query = _db.Products.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();
                query = query.Where(p =>
                    p.Title.Contains(term) ||
                    p.Brand.Contains(term) ||
                    p.Model.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(brand))
            {
                var b = brand.Trim();
                query = query.Where(p => p.Brand == b);
            }

            if (!string.IsNullOrWhiteSpace(condition))
            {
                // aceita "new"/"used" etc
                if (Enum.TryParse<ProductCondition>(condition, true, out var parsed))
                    query = query.Where(p => p.Condition == parsed);
            }

            // ordenação default "marketplace": mais bem avaliados primeiro, depois título
            query = query
                .OrderByDescending(p => p.Rating != null ? p.Rating.Average : 0m)
                .ThenBy(p => p.Title);


            var baseQuery = query;

            var countTask = baseQuery.CountAsync(ct);

            var itemsTask = baseQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            await Task.WhenAll(countTask, itemsTask);

            return (await itemsTask, await countTask);
        }

        public async Task AddAsync(Product product, CancellationToken ct)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Product product, CancellationToken ct)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync(ct);
        }

    }
}
