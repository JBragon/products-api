namespace Products.Domain.Entities.Products
{
    public sealed class Money
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; } = "BRL";

        protected Money() { } // EF

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }
    }
}
