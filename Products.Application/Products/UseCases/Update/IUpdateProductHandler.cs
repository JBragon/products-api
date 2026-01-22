namespace Products.Application.Products.UseCases.Update
{
    public interface IUpdateProductHandler
    {
        Task HandleAsync(UpdateProductCommand command, CancellationToken ct);
    }
}
