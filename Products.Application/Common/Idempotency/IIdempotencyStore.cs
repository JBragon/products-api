namespace Products.Application.Common.Idempotency
{
    public interface IIdempotencyStore
    {
        Task<Guid?> GetAsync(string key, CancellationToken ct);
        Task StoreAsync(string key, Guid productId, CancellationToken ct);
    }
}
