namespace Products.Domain.Entities.Products
{
    public sealed class ProductAttribute
    {
        public string Name { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;

        protected ProductAttribute() { } // EF

        public ProductAttribute(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
