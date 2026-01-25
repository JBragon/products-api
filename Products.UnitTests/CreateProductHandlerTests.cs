using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Products.Application.Products.Ports;
using Products.Application.Products.UseCases.Create;
using Products.Domain.Entities.Products;

namespace Products.UnitTests;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IIdempotencyStore> _idempotencyStoreMock;
    private readonly Mock<ILogger<CreateProductHandler>> _loggerMock;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _idempotencyStoreMock = new Mock<IIdempotencyStore>();
        _loggerMock = new Mock<ILogger<CreateProductHandler>>();
        _handler = new CreateProductHandler(_repositoryMock.Object, _idempotencyStoreMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnExistingId_WhenIdempotencyKeyExists()
    {
        // Arrange
        var key = "existing-key";
        var existingId = Guid.NewGuid();
        _idempotencyStoreMock
            .Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingId);

        var command = CreateCommand();

        // Act
        var result = await _handler.HandleAsync(command, key, CancellationToken.None);

        // Assert
        result.Should().Be(existingId);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _idempotencyStoreMock.Verify(x => x.StoreAsync(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateProduct_WhenIdempotencyKeyDoesNotExist()
    {
        // Arrange
        var key = "new-key";
        _idempotencyStoreMock
            .Setup(x => x.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        var command = CreateCommand();

        // Act
        var result = await _handler.HandleAsync(command, key, CancellationToken.None);

        // Assert
        result.Should().Be(command.ProductId);
        
        _repositoryMock.Verify(x => x.AddAsync(
            It.Is<Product>(p => 
                p.Id == command.ProductId &&
                p.Title == command.Title &&
                p.Price.Amount == command.Price &&
                p.Stock.AvailableQuantity == command.Stock &&
                p.Attributes.Count == 1 &&
                p.Pictures.Count == 1), 
            It.IsAny<CancellationToken>()), Times.Once);

        _idempotencyStoreMock.Verify(x => x.StoreAsync(key, command.ProductId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private CreateProductCommand CreateCommand()
    {
        return new CreateProductCommand(
            Guid.NewGuid(),
            "Test Title",
            "Test Brand",
            "Test Model",
            "New",
            150.0m,
            "USD",
            10,
            "Description",
            new List<(string Name, string Value)> { ("Color", "Blue") },
            new List<string> { "http://image.com/1.jpg" }
        );
    }
}
