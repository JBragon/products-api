using Products.Application.Products.Dtos;

namespace Products.Application.Products.UseCases.Query.GetDetail
{
    /// <summary>
    /// Contract for retrieving product details.
    /// </summary>
    public interface IGetProductDetailHandler
    {
        Task<ProductDetailDto?> HandleAsync(Guid productId, CancellationToken ct);
    }
}
