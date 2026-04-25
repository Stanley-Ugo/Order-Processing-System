using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Common;
using OrderProcessingSystem.Application.Common.Interfaces;
using OrderProcessingSystem.Application.Orders.DTOs;

namespace OrderProcessingSystem.Application.Orders.Queries.GetOrder
{
    public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Result<OrderDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetOrderQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<OrderDto>> Handle(GetOrderQuery request, CancellationToken ct)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct);

            if (order == null)
                return Result<OrderDto>.Failure("Order not found");

            var dto = new OrderDto(
                order.Id,
                order.CustomerId,
                order.Status.ToString(),
                order.TotalAmount,
                order.CreatedAt,
                order.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    i.Product.Sku,
                    i.Product.Name,
                    i.Quantity,
                    i.UnitPrice,
                    i.UnitPrice * i.Quantity)).ToList()
            );

            return Result<OrderDto>.Success(dto);
        }
    }
}
