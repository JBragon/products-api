namespace Products.Application.Products.Dtos
{
    public sealed record ShippingDto(
        bool FreeShipping,
        DateTime? EstimatedDeliveryDate
    );
}
