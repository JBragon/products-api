using Microsoft.AspNetCore.Mvc;
using Products.Api.Contracts;
using Products.Api.Filters;
using Products.Application.Products.Queries;
using Products.Application.Products.UseCases.Create;
using Products.Application.Products.UseCases.Delete;
using Products.Application.Products.UseCases.Query.GetDetail;
using Products.Application.Products.UseCases.Query.Search;
using Products.Application.Products.UseCases.Update;

namespace Products.Api.Controllers
{
    /// <summary>
    /// API Controller exposing endpoints for product management.
    /// Acts as an entry point, delegating logic to Application Use Cases via specific Handlers.
    /// </summary>
    [ApiController]
    [Route("api/products")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly IGetProductDetailHandler _getProductDetailHandler;
        private readonly ISearchProductsHandler _searchProductsHandler;
        private readonly ICreateProductHandler _createProductHandler;
        private readonly IUpdateProductHandler _updateProductHandler;
        private readonly IInactivateProductHandler _inactivateProductHandler;

        public ProductsController(
            IGetProductDetailHandler getProductDetailHandler,
            ISearchProductsHandler searchProductsHandler,
            ICreateProductHandler createProductHandler,
            IUpdateProductHandler updateProductHandler,
            IInactivateProductHandler inactivateProductHandler)
        {
            _getProductDetailHandler = getProductDetailHandler;
            _searchProductsHandler = searchProductsHandler;
            _createProductHandler = createProductHandler;
            _updateProductHandler = updateProductHandler;
            _inactivateProductHandler = inactivateProductHandler;
        }

        /// <summary>
        /// Retrieves a product by its unique identifier (GUID).
        /// Triggers the GetProductDetail use case.
        /// </summary>
        /// <param name="id">The product ID (GUID).</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>The product details if found; otherwise, 404.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest(new { error = "Invalid product id. Must be a GUID." });

            var result = await _getProductDetailHandler.HandleAsync(guid, ct);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>
        /// Lists products with support for pagination, filtering, and text search.
        /// Triggers the SearchProducts use case.
        /// </summary>
        /// <param name="q">Optional text search term.</param>
        /// <param name="brand">Filter by exact brand name.</param>
        /// <param name="condition">Filter by condition (New, Used, etc).</param>
        /// <param name="page">Page number (default 1).</param>
        /// <param name="pageSize">Items per page (default 10).</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>A paged list of products.</returns>
        [HttpGet]
        public async Task<IActionResult> GetList
        (
            [FromQuery] string? q,
            [FromQuery] string? brand,
            [FromQuery] string? condition,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken ct = default
        )
        {
            var result = await _searchProductsHandler.HandleAsync(
                new ProductListQuery(q, brand, condition, page, pageSize),
                ct
            );

            return Ok(result);
        }

        /// <summary>
        /// Creates a new product.
        /// Requires Idempotency-Key header to guarantee safe retries.
        /// Triggers the CreateProduct use case.
        /// </summary>
        /// <param name="request">The product creation payload.</param>
        /// <param name="idempotencyKey">Unique key (UUID) to ensure idempotency.</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>The created product's resource location.</returns>
        [HttpPost]
        [ServiceFilter(typeof(FluentValidationFilter<ProductCreateRequest>))]
        public async Task<IActionResult> Create(
            [FromBody] ProductCreateRequest request,
            [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(idempotencyKey))
                return BadRequest("Idempotency-Key header is required.");

            var command = new CreateProductCommand(
                ProductId: Guid.NewGuid(),
                Title: request.Title,
                Brand: request.Brand,
                Model: request.Model,
                Condition: request.Condition,
                Price: request.Price,
                Currency: request.Currency,
                Stock: request.Stock,
                Description: request.Description,
                Attributes: request.Attributes?
                    .Select(a => (a.Name, a.Value))
                    .ToList() ?? [],
                Pictures: request.Pictures?
                    .Select(p => p.Url)
                    .ToList() ?? []
            );

            var productId = await _createProductHandler.HandleAsync(
                command,
                idempotencyKey,
                ct
            );

            return CreatedAtAction(nameof(GetById), new { id = productId }, null);
        }

        /// <summary>
        /// Fully updates an existing product.
        /// Triggers the UpdateProduct use case.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="request">The update payload.</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>No Content (204) if successful.</returns>
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(FluentValidationFilter<ProductUpdateRequest>))]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] ProductUpdateRequest request,
            CancellationToken ct)
        {
            var command = new UpdateProductCommand(
                ProductId: id,
                Title: request.Title,
                Brand: request.Brand,
                Model: request.Model,
                Condition: request.Condition,
                Price: request.Price,
                Currency: request.Currency,
                Stock: request.Stock,
                Description: request.Description,
                Attributes: request.Attributes?
                    .Select(a => (a.Name, a.Value))
                    .ToList() ?? [],
                Pictures: request.Pictures?
                    .Select(p => p.Url)
                    .ToList() ?? []
            );

            await _updateProductHandler.HandleAsync(command, ct);

            return NoContent();
        }

        /// <summary>
        /// Inactivates (soft deletes) a product.
        /// Triggers the InactivateProduct use case.
        /// </summary>
        /// <param name="id">The product ID.</param>
        /// <param name="ct">Cancellation Token.</param>
        /// <returns>No Content (204) if successful or if product was already inactive.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _inactivateProductHandler.HandleAsync(
                new InactivateProductCommand(id),
                ct
            );

            return NoContent();
        }
    }
}
