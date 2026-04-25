using MediatR;
using OrderProcessingSystem.Application.Common;
using OrderProcessingSystem.Application.Orders.DTOs;

namespace OrderProcessingSystem.Application.Orders.Queries.GetOrder
{
    public record GetOrderQuery(Guid OrderId) : IRequest<Result<OrderDto>>;
}
