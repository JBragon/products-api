namespace Products.Application.Products.Dtos
{
    public sealed record ReturnsDto(
        bool Allowed,
        int WindowDays
    );
}
