using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Common;
using OrderProcessingSystem.Application.Common.Interfaces;
using OrderProcessingSystem.Application.Orders.DTOs;
using OrderProcessingSystem.Domain.Common;
using OrderProcessingSystem.Domain.Entities;
using OrderProcessingSystem.Domain.Events;
using System.Data;
using System.Text.Json;

namespace OrderProcessingSystem.Application.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Result<OrderDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTime _dateTime;

        public PlaceOrderCommandHandler(IApplicationDbContext context, IDateTime dateTime)
        {
            _context = context;
            _dateTime = dateTime;
        }

        public async Task<Result<OrderDto>> Handle(PlaceOrderCommand request, CancellationToken ct)
        {
            var existingOrder = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.IdempotencyKey == request.IdempotencyKey, ct);

            if (existingOrder != null)
                return Result<OrderDto>.Success(MapToDto(existingOrder));

            await using var tx = await _context.BeginTransactionAsync(IsolationLevel.Serializable, ct);

            try
            {
                var productIds = request.Items.Select(i => i.ProductId).ToList();

                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync(ct);

                var orderItems = new List<OrderItem>();
                decimal total = 0;

                foreach (var item in request.Items)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product == null)
                        return Result<OrderDto>.Failure($"Product {item.ProductId} not found");

                    if (product.Stock < item.Quantity)
                        return Result<OrderDto>.Failure($"Insufficient stock for {product.Sku}. Available: {product.Stock}");

                    product.Stock -= item.Quantity;

                    var orderItem = new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.UnitPrice
                    };
                    orderItems.Add(orderItem);
                    total += orderItem.UnitPrice * orderItem.Quantity;
                }

                var order = Order.Create(request.CustomerId, orderItems, request.IdempotencyKey, total, _dateTime.UtcNow);
                await _context.Orders.AddAsync(order, ct);

                var orderPlacedEvent = new OrderPlacedEvent(
                    order.Id,
                    order.CustomerId,
                    order.TotalAmount,
                    order.CreatedAt,
                    order.Items.Select(i => new OrderItemEvent(
                        i.ProductId,
                        products.First(p => p.Id == i.ProductId).Sku,
                        i.Quantity,
                        i.UnitPrice)).ToList()
                );

                var outboxMessage = new OutboxMessage
                {
                    Id = Guid.NewGuid(),
                    OccurredOn = _dateTime.UtcNow,
                    Type = nameof(OrderPlacedEvent),
                    Content = JsonSerializer.Serialize(orderPlacedEvent)
                };

                await _context.OutboxMessages.AddAsync(outboxMessage, ct);
                await _context.SaveChangesAsync(ct);

                await tx.CommitAsync(ct);

                return Result<OrderDto>.Success(MapToDto(order));
            }
            catch (DbUpdateConcurrencyException)
            {
                await tx.RollbackAsync(ct);
                return Result<OrderDto>.Failure("Stock was modified by another transaction. Please retry.");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(ct);
                return Result<OrderDto>.Failure($"Order processing failed: {ex.Message}");
            }
        }

        private static OrderDto MapToDto(Order order) =>
            new(
                order.Id,
                order.CustomerId,
                order.Status.ToString(),
                order.TotalAmount,
                order.CreatedAt,
                order.Items.Select(i => new OrderItemDto(
                    i.ProductId,
                    string.Empty,
                    string.Empty,
                    i.Quantity,
                    i.UnitPrice,
                    i.UnitPrice * i.Quantity))
                .ToList()
            );
    }
}
