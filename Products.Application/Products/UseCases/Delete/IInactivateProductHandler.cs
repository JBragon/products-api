namespace Products.Application.Products.UseCases.Delete
{
    /// <summary>
    /// Contract for product inactivation (soft delete) logic.
    /// </summary>
    public interface IInactivateProductHandler
    {
        Task HandleAsync(InactivateProductCommand command, CancellationToken ct);
    }
}
