using OrderProcessingSystem.Domain.Common;
using OrderProcessingSystem.Domain.Enums;

namespace OrderProcessingSystem.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public string IdempotencyKey { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<DomainEvent> DomainEvents { get; } = new();

        public static Order Create(Guid customerId, List<OrderItem> orderItems, string idempotencyKey, decimal total, DateTime utcNow)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Status = OrderStatus.Pending,
                TotalAmount = total,
                Items = orderItems,
                IdempotencyKey = idempotencyKey,
                CreatedAt = utcNow
            };
        }
    }
}
