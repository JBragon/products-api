namespace Products.Application.Products.UseCases.Query.Search
{
    public sealed record ProductListQuery (
        string? Q,
        string? Brand,
        string? Condition,
        int Page,
        int PageSize
    );
}
