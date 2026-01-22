
namespace Products.Application.Products.Services.Command
{
    public interface IInactivateProductHandler
    {
        Task HandleAsync(InactivateProductCommand command, CancellationToken ct);
    }
}
