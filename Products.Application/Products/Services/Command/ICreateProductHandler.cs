using Products.Application.Products.Services.Command;

namespace Products.Application.Products.Services.Query
{
    public interface ICreateProductHandler
    {
        Task<Guid> HandleAsync(CreateProductCommand command, string idempotencyKey, CancellationToken ct);
    }
}
