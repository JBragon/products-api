using AutoMapper;
using FluentAssertions;
using Moq;
using Products.Application.Common.Caching;
using Products.Application.Products.Dtos;
using Products.Application.Products.Ports;
using Products.Application.Products.UseCases.Query.GetDetail;
using Products.Domain.Entities.Products;
using Xunit;

namespace Products.UnitTests;

public class GetProductDetailHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IProductCache> _cacheMock;
    private readonly GetProductDetailHandler _handler;

    public GetProductDetailHandlerTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _cacheMock = new Mock<IProductCache>();
        _handler = new GetProductDetailHandler(_repositoryMock.Object, _mapperMock.Object, _cacheMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnCachedResult_WhenCacheHit()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var cachedDto = CreateDto(productId);
        _cacheMock.Setup(x => x.GetAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedDto);

        // Act
        var result = await _handler.HandleAsync(productId, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(cachedDto);
        _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnNull_WhenCacheMiss_AndProductNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _cacheMock.Setup(x => x.GetAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDetailDto?)null);
        _repositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.HandleAsync(productId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _cacheMock.Verify(x => x.SetAsync(It.IsAny<Guid>(), It.IsAny<ProductDetailDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnDto_AndCacheIt_WhenCacheMiss_AndProductFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product(productId, "T", "B", "M", ProductCondition.New, new Money(10, "BRL"), new Stock(1));
        var dto = CreateDto(productId);

        _cacheMock.Setup(x => x.GetAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductDetailDto?)null);
        _repositoryMock.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mapperMock.Setup(x => x.Map<ProductDetailDto>(product))
            .Returns(dto);

        // Act
        var result = await _handler.HandleAsync(productId, CancellationToken.None);

        // Assert
        result.Should().BeSameAs(dto);
        _cacheMock.Verify(x => x.SetAsync(productId, dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    private ProductDetailDto CreateDto(Guid id)
    {
        return new ProductDetailDto(
            id, "Title", "Brand", "Model", "new", 10m, "USD", null, 10,
            new List<string>(), new List<string>(), new List<ProductAttributeDto>(),
            null, null, false, null, "Desc");
    }
}
