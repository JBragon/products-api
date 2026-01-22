namespace Products.Domain.Entities.Products
{
    public sealed class Installments
    {
        public int Quantity { get; private set; }
        public decimal Amount { get; private set; }
        public bool InterestFree { get; private set; }

        protected Installments() { } // EF

        public Installments(int quantity, decimal amount, bool interestFree)
        {
            Quantity = quantity;
            Amount = amount;
            InterestFree = interestFree;
        }
    }
}
