namespace Products.Domain.Entities.Products
{
    public sealed class ShippingInfo
    {
        public bool FreeShipping { get; private set; }
        public DateTime? EstimatedDeliveryDate { get; private set; }

        protected ShippingInfo() { } // EF

        public ShippingInfo(bool freeShipping, DateTime? estimatedDeliveryDate)
        {
            FreeShipping = freeShipping;
            EstimatedDeliveryDate = estimatedDeliveryDate;
        }
    }
}
