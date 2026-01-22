namespace Products.Application.Common.Paging
{
    public sealed record PagedResult<T>(
        IReadOnlyCollection<T> Items,
        int Page,
        int PageSize,
        int TotalItems
    );
}
