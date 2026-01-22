namespace Products.Application.Products.Dtos
{
    public sealed record ProductDetailDto(
       Guid Id,
       string Title,
       string Brand,
       string Model,
       string Condition,

       decimal Price,
       string Currency,
       InstallmentsDto? Installments,

       int AvailableQuantity,

       IReadOnlyCollection<string> Pictures,
       IReadOnlyCollection<string> Highlights,
       IReadOnlyCollection<ProductAttributeDto> Attributes,

       ShippingDto? Shipping,
       ReturnsDto? Returns,
       bool PurchaseProtection,
       RatingDto? Rating,

       string? Description
   );
}
