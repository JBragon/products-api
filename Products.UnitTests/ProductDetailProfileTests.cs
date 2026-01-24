using AutoMapper;
using FluentAssertions;
using Products.Application.Products.Dtos;
using Products.Application.Products.Mapping;
using Products.Domain.Entities.Products;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Products.UnitTests;

public class ProductDetailProfileTests
{
    private readonly IMapper _mapper;
    private readonly MapperConfiguration _configuration;



    public ProductDetailProfileTests()
    {
        _configuration = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile(new ProductDetailProfile());
        }, NullLoggerFactory.Instance);
        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    public void Configuration_ShouldBeValid()
    {
        _configuration.AssertConfigurationIsValid();
    }

    [Fact]
    public void ShouldMap_Product_To_ProductDetailDto()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(), "Title", "Brand", "Model",
            ProductCondition.New, new Money(123.45m, "USD"), new Stock(50),
            attributes: new[] { new ProductAttribute("Key", "Value") },
            pictures: new[] { new ProductPicture("http://img.com") },
            rating: new Rating(4.8m, 100),
            description: "Desc");

        // Act
        var dto = _mapper.Map<ProductDetailDto>(product);

        // Assert
        dto.Id.Should().Be(product.Id);
        dto.Title.Should().Be("Title");
        dto.Price.Should().Be(123.45m);
        dto.Currency.Should().Be("USD");
        dto.Condition.Should().Be("new");
        dto.AvailableQuantity.Should().Be(50);
        dto.Description.Should().Be("Desc");
        
        dto.Attributes.Should().HaveCount(1);
        dto.Attributes.First().Name.Should().Be("Key");
        dto.Pictures.Should().HaveCount(1).And.Contain("http://img.com");
        
        dto.Rating.Should().NotBeNull();
        dto.Rating.Average.Should().Be(4.8m);
    }
}
