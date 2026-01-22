namespace Products.Domain.Entities.Products
{
    public sealed class ProductPicture
    {
        public string Url { get; private set; } = string.Empty;

        protected ProductPicture() { } // EF

        public ProductPicture(string url)
        {
            Url = url;
        }
    }
}
