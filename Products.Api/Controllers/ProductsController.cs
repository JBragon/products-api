using Microsoft.AspNetCore.Mvc;
using Products.Application.Products.Services;

namespace Products.Api.Controllers
{
    [ApiController]
    [Route("api/products")]
    public sealed class ProductsController : ControllerBase
    {
        private readonly IProductQueryService _service;

        public ProductsController(IProductQueryService service)
            => _service = service;

        // GET /api/products/{id}
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string id, CancellationToken ct)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest(new { error = "Invalid product id. Must be a GUID." });

            var result = await _service.GetDetailAsync(guid, ct);
            return result is null ? NotFound() : Ok(result);
        }
    }
}
