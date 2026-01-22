using Microsoft.EntityFrameworkCore;
using Products.Application.Ports;
using Products.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Infrastructure.Repositories
{
    public sealed class ProductRepository : IProductRepository
    {
        private readonly DatabaseContext _db;

        public ProductRepository(DatabaseContext db) => _db = db;

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            // Owned collections (Pictures/Attributes) normalmente são carregadas junto.
            // Mesmo assim, usar AsNoTracking é bom para leitura.
            return _db.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }
    }
}
