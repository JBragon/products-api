namespace Products.Application.Products.Queries
{
    public sealed record ProductListQuery (
        string? Q,
        string? Brand,
        string? Condition,
        int Page,
        int PageSize
    );
}
