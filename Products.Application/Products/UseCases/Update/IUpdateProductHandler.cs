namespace Products.Application.Products.UseCases.Update
{
    /// <summary>
    /// Contract for product update logic.
    /// </summary>
    public interface IUpdateProductHandler
    {
        Task HandleAsync(UpdateProductCommand command, CancellationToken ct);
    }
}
