namespace Products.Application.Products.Dtos
{
    public sealed record RatingDto(
         decimal Average,
         int TotalReviews
     );
}
