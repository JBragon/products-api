namespace Products.Application.Products.Dtos
{
    public sealed record ProductListItemDto(
        Guid Id,
        string Title,
        string Brand,
        string Model,
        string Condition,
        decimal Price,
        string Currency,
        string? ThumbnailUrl,
        int AvailableQuantity,
        decimal? RatingAverage,
        int? RatingTotalReviews
    );
}
