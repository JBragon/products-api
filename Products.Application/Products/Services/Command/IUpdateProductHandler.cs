namespace Products.Application.Products.Services.Command
{
    public interface IUpdateProductHandler
    {
        Task HandleAsync(UpdateProductCommand command, CancellationToken ct);
    }
}
