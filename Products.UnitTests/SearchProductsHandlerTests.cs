using FluentAssertions;
using Microsoft.Extensions.Logging;
using System.Linq;
using Moq;
using Products.Application.Common.Paging;
using Products.Application.Products.Dtos;
using Products.Application.Products.Ports;
using Products.Application.Products.Queries;
using Products.Application.Products.UseCases.Query.Search;
using Products.Domain.Entities.Products;
using Xunit;

namespace Products.UnitTests;

public class SearchProductsHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<ILogger<SearchProductsHandler>> _loggerMock;
    private readonly SearchProductsHandler _handler;

    public SearchProductsHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<SearchProductsHandler>>();
        _handler = new SearchProductsHandler(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldClampPageAndPageSize()
    {
        // Arrange
        var query = new ProductListQuery(null, null, null, 0, 0); // Invalid page/size
        _repositoryMock
            .Setup(x => x.SearchAsync(
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product>(), 0));

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.SearchAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            1, // Page clamped to 1
            10, // PageSize clamped to 10
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldLimitMaxPageSize()
    {
        // Arrange
        var query = new ProductListQuery(null, null, null, 1, 999);
        _repositoryMock
            .Setup(x => x.SearchAsync(null, null, null, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product>(), 0));

        // Act
        await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        _repositoryMock.Verify(x => x.SearchAsync(
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int>(),
            50, // Clamped to 50
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldMapProductsToDtos()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(), "Title", "Brand", "Model",
            ProductCondition.New, new Money(100, "BRL"), new Stock(5),
            rating: new Rating(4.5m, 10),
            pictures: new[] { new ProductPicture("http://thumb.com/1.jpg") });

        _repositoryMock
            .Setup(x => x.SearchAsync(null, null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product> { product }, 1));

        var query = new ProductListQuery(null, null, null, 1, 10);

        // Act
        var result = await _handler.HandleAsync(query, CancellationToken.None);

        // Assert
        result.Items.Should().HaveCount(1);
        var dto = result.Items.First();
        dto.Title.Should().Be("Title");
        dto.Price.Should().Be(100);
        dto.Currency.Should().Be("BRL");
        dto.ThumbnailUrl.Should().Be("http://thumb.com/1.jpg");
        dto.RatingAverage.Should().Be((decimal?)4.5m);
        dto.Condition.Should().Be("new"); // Lowercase check
    }
}
