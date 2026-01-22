namespace Products.Application.Products.UseCases.Create
{
    public sealed record CreateProductCommand(
        Guid ProductId,
        string Title,
        string Brand,
        string Model,
        string Condition,
        decimal Price,
        string Currency,
        int Stock,
        string? Description,
        IReadOnlyCollection<(string Name, string Value)> Attributes,
        IReadOnlyCollection<string> Pictures
    );
}
