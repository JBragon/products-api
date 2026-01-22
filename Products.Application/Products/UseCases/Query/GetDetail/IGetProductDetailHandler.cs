using Products.Application.Products.Dtos;

namespace Products.Application.Products.UseCases.Query.GetDetail
{
    public interface IGetProductDetailHandler
    {
        Task<ProductDetailDto?> HandleAsync(Guid productId, CancellationToken ct);
    }
}
