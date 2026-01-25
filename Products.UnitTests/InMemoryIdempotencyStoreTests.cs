using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Products.Infrastructure.Idempotency;

namespace Products.UnitTests;

public class InMemoryIdempotencyStoreTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly InMemoryIdempotencyStore _store;
    private readonly ILogger<InMemoryIdempotencyStore> _logger;

    public InMemoryIdempotencyStoreTests()
    {
        _logger = new Logger<InMemoryIdempotencyStore>(new LoggerFactory());
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _store = new InMemoryIdempotencyStore(_memoryCache, _logger);
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
