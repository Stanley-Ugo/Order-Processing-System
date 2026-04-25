using MediatR;
using OrderProcessingSystem.Application.Common;
using OrderProcessingSystem.Application.Orders.DTOs;

namespace OrderProcessingSystem.Application.Orders.Commands.PlaceOrder
{
    public record PlaceOrderItemDto(Guid ProductId, int Quantity);
    public record PlaceOrderCommand(Guid CustomerId, List<PlaceOrderItemDto> Items, string IdempotencyKey) : IRequest<Result<OrderDto>>;
}
