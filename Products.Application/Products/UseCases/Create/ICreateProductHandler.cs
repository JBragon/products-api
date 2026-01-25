namespace Products.Application.Products.UseCases.Create
{
    /// <summary>
    /// Contract for product creation logic.
    /// </summary>
    public interface ICreateProductHandler
    {
        Task<Guid> HandleAsync(CreateProductCommand command, string idempotencyKey, CancellationToken ct);
    }
}
