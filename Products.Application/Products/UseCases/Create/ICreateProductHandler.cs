namespace Products.Application.Products.UseCases.Create
{
    public interface ICreateProductHandler
    {
        Task<Guid> HandleAsync(CreateProductCommand command, string idempotencyKey, CancellationToken ct);
    }
}
