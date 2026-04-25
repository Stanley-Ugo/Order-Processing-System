using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Application.Products.Queries.GetProductBySku;

namespace OrderProcessingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ISender _mediator;

        public ProductsController(ISender mediator) => _mediator = mediator;

        [HttpGet("{sku}")]
        public async Task<IActionResult> GetBySku(string sku, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetProductBySkuQuery(sku), ct);
            return result.Succeeded ? Ok(result.Data) : NotFound();
        }
    }
}
