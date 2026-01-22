namespace Products.Application.Products.Dtos
{
    public sealed record InstallmentsDto(
        int Quantity,
        decimal Amount,
        bool InterestFree
    );
}
