using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Products.Application.Common.Caching;
using Products.Application.Common.Exceptions;
using Products.Application.Products.Ports;
using Products.Application.Products.UseCases.Delete;
using Products.Domain.Entities.Products;
using Xunit;

namespace Products.UnitTests;

public class InactivateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IProductCache> _cacheMock;
    private readonly Mock<ILogger<InactivateProductHandler>> _loggerMock;
    private readonly InactivateProductHandler _handler;

    public InactivateProductHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _cacheMock = new Mock<IProductCache>();
        _loggerMock = new Mock<ILogger<InactivateProductHandler>>();
        _handler = new InactivateProductHandler(_repositoryMock.Object, _cacheMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var command = new InactivateProductCommand(Guid.NewGuid());
        _repositoryMock
            .Setup(x => x.GetByIdForUpdateAsync(command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var act = () => _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>();
    }

    [Fact]
    public async Task HandleAsync_ShouldInactivateProduct_WhenFound()
    {
        // Arrange
        var command = new InactivateProductCommand(Guid.NewGuid());
        var product = new Product(
            command.ProductId, "Title", "Brand", "Model",
            ProductCondition.New, new Money(100, "BRL"), new Stock(10));

        _repositoryMock
            .Setup(x => x.GetByIdForUpdateAsync(command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        product.IsActive.Should().BeFalse();
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(x => x.InvalidateAsync(command.ProductId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
