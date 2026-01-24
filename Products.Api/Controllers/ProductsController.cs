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
