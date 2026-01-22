namespace Products.Domain.Entities.Products
{
    public sealed class Stock
    {
        public int AvailableQuantity { get; private set; }

        protected Stock() { } // EF

        public Stock(int availableQuantity)
        {
            AvailableQuantity = availableQuantity;
        }
    }
}
