using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Application.Common;
using OrderProcessingSystem.Application.Orders.Commands.PlaceOrder;
using OrderProcessingSystem.Application.Orders.DTOs;
using OrderProcessingSystem.Application.Orders.Queries.GetOrder;

namespace OrderProcessingSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ISender _mediator;

        public OrdersController(ISender mediator) => _mediator = mediator;

        /// <summary>
        /// Place a new order. Idempotent via Idempotency-Key header.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PlaceOrder(
            [FromBody] PlaceOrderRequest request,
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey,
            CancellationToken ct)
        {
            var command = new PlaceOrderCommand(
                request.CustomerId,
                request.Items.Select(i => new PlaceOrderItemDto(i.ProductId, i.Quantity)).ToList(),
                idempotencyKey);

            var result = await _mediator.Send(command, ct);

            return result.Succeeded
               ? CreatedAtAction(nameof(GetOrder), new { id = result.Data.Id }, result.Data)
                : HandleFailure(result);
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetOrderQuery(id), ct);
            return result.Succeeded ? Ok(result.Data) : NotFound(result.Errors);
        }

        private IActionResult HandleFailure<T>(Result<T> result)
        {
            if (result.Errors.Any(e => e.Contains("Insufficient stock") || e.Contains("modified by another")))
                return Conflict(new ProblemDetails { Title = "Conflict", Detail = string.Join("; ", result.Errors) });

            return BadRequest(new ProblemDetails { Title = "Validation Error", Detail = string.Join("; ", result.Errors) });
        }
    }

    public record PlaceOrderRequest(Guid CustomerId, List<PlaceOrderItemRequest> Items);
    public record PlaceOrderItemRequest(Guid ProductId, int Quantity);
}
