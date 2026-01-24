using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Products.Infrastructure.Idempotency;
using Xunit;

namespace Products.UnitTests;

public class InMemoryIdempotencyStoreTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly InMemoryIdempotencyStore _store;

    public InMemoryIdempotencyStoreTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _store = new InMemoryIdempotencyStore(_memoryCache);
    }

    [Fact]
    public async Task ShouldStoreAndRetrieveKey()
    {
        // Arrange
        var key = "test-key";
        var id = Guid.NewGuid();

        // Act
        await _store.StoreAsync(key, id, CancellationToken.None);
        var retrieved = await _store.GetAsync(key, CancellationToken.None);

        // Assert
        retrieved.Should().Be(id);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenKeyDoesNotExist()
    {
        // Act
        var retrieved = await _store.GetAsync("non-existent", CancellationToken.None);

        // Assert
        retrieved.Should().BeNull();
    }
}
