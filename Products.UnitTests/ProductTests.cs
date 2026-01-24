using FluentAssertions;
using Products.Domain.Entities.Products;
using Xunit;

namespace Products.UnitTests;

public class ProductTests
{
    [Fact]
    public void Inactivate_ShouldSetIsActiveToFalse_WhenActive()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(), "Title", "Brand", "Model",
            ProductCondition.New, new Money(100, "BRL"), new Stock(10));

        // Act
        product.Inactivate();

        // Assert
        product.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Inactivate_ShouldRemainFalse_WhenAlreadyInactive()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(), "Title", "Brand", "Model",
            ProductCondition.New, new Money(100, "BRL"), new Stock(10));
        product.Inactivate(); // First call

        // Act
        product.Inactivate(); // Second call

        // Assert
        product.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Update_ShouldUpdateFields_AndReplaceLists()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(), "OldTitle", "OldBrand", "OldModel",
            ProductCondition.New, new Money(100, "BRL"), new Stock(10),
            attributes: new[] { new ProductAttribute("Color", "Red") },
            pictures: new[] { new ProductPicture("url1") });

        var newAttributes = new List<ProductAttribute> { new ProductAttribute("Size", "M") };
        var newPictures = new List<ProductPicture> { new ProductPicture("url2") };
        var newPrice = new Money(200, "USD");
        var newStock = new Stock(5);

        // Act
        product.Update(
            "NewTitle", "NewBrand", "NewModel",
            ProductCondition.Used, newPrice, newStock, "NewDesc",
            newAttributes, newPictures);

        // Assert
        product.Title.Should().Be("NewTitle");
        product.Brand.Should().Be("NewBrand");
        product.Model.Should().Be("NewModel");
        product.Condition.Should().Be(ProductCondition.Used);
        product.Price.Should().Be(newPrice);
        product.Stock.Should().Be(newStock);
        product.Description.Should().Be("NewDesc");

        product.Attributes.Should().HaveCount(1).And.Contain(x => x.Name == "Size");
        product.Pictures.Should().HaveCount(1).And.Contain(x => x.Url == "url2");
    }
}
