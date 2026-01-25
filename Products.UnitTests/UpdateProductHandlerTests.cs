using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Products.Application.Common.Caching;
using Products.Application.Common.Exceptions;
using Products.Application.Products.Ports;
using Products.Application.Products.UseCases.Update;
using Products.Domain.Entities.Products;
using Xunit;

namespace Products.UnitTests;

public class UpdateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IProductCache> _cacheMock;
    private readonly Mock<ILogger<UpdateProductHandler>> _loggerMock;
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _cacheMock = new Mock<IProductCache>();
        _loggerMock = new Mock<ILogger<UpdateProductHandler>>();
        _handler = new UpdateProductHandler(_repositoryMock.Object, _cacheMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var command = CreateCommand();
        _repositoryMock
            .Setup(x => x.GetByIdForUpdateAsync(command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var act = () => _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>();
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _cacheMock.Verify(x => x.InvalidateAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldUpdateProduct_WhenFound()
    {
        // Arrange
        var command = CreateCommand();
        var product = new Product(
            command.ProductId, "Old", "Old", "Old", ProductCondition.New,
            new Money(10, "BRL"), new Stock(0));

        _repositoryMock
            .Setup(x => x.GetByIdForUpdateAsync(command.ProductId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        product.Title.Should().Be(command.Title);
        product.Price.Amount.Should().Be(command.Price);
        
        _repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cacheMock.Verify(x => x.InvalidateAsync(command.ProductId, It.IsAny<CancellationToken>()), Times.Once);
    }

    private UpdateProductCommand CreateCommand()
    {
        return new UpdateProductCommand(
            Guid.NewGuid(),
            "Title", "Brand", "Model", "Used",
            100m, "USD", 5, "Desc",
            new List<(string, string)>(),
            new List<string>()
        );
    }
}
