using System.Net;
using Xunit;

namespace Products.IntegrationTests
{
    public class Products_GetDetail_IntegrationTests : IntegrationTestBase
    {
        public Products_GetDetail_IntegrationTests(ApiFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Get_ShouldReturn200_WhenProductExists()
        {
            // Arrange: Get a valid ID from the list
            var listResponse = await _client.GetAsync("/api/products?page=1&pageSize=1");
            listResponse.EnsureSuccessStatusCode();
            var listContent = await DeserializeResponse<ProductListResponse>(listResponse);
            
            Assert.NotNull(listContent);
            Assert.NotEmpty(listContent.Items);
            var existingId = listContent.Items[0].Id;

            // Act
            var response = await _client.GetAsync($"/api/products/{existingId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var product = await DeserializeResponse<ProductDetailResponse>(response);
            Assert.NotNull(product);
            Assert.Equal(existingId, product.Id);
            Assert.NotNull(product.Title);
            Assert.True(product.Price > 0); 
            Assert.NotNull(product.Currency);
             // Verify attributes and pictures presence (can be empty arrays)
            Assert.NotNull(product.Attributes);
            Assert.NotNull(product.Pictures);
        }

        [Fact]
        public async Task Get_ShouldReturn404_WhenProductDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Get_ShouldReturn400_WhenIdIsInvalid()
        {
            // Act
            var response = await _client.GetAsync("/api/products/not-a-guid");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    // Helper classes for deserialization (internal to tests or could be shared)
    public class ProductListResponse
    {
        public List<ProductItemResponse> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ProductItemResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
    }

    public class ProductDetailResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int AvailableQuantity { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public List<ProductAttributeResponse> Attributes { get; set; } = new(); 
        public List<string> Pictures { get; set; } = new();
    }

    public class ProductAttributeResponse
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
