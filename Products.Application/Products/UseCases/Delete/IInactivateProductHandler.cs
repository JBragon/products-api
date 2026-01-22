namespace Products.Application.Products.UseCases.Delete
{
    public interface IInactivateProductHandler
    {
        Task HandleAsync(InactivateProductCommand command, CancellationToken ct);
    }
}
