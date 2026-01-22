namespace Products.Application.Common.Exceptions
{
    public sealed class ProductNotFoundException : Exception
    {
        public Guid ProductId { get; }

        public ProductNotFoundException(Guid productId)
            : base($"Product '{productId}' was not found.")
        {
            ProductId = productId;
        }
    }
}
