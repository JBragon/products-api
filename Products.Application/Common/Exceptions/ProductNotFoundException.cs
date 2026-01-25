namespace Products.Application.Common.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested product cannot be found.
    /// Automatically mapped to HTTP 404 by the ExceptionHandlingMiddleware.
    /// </summary>
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
