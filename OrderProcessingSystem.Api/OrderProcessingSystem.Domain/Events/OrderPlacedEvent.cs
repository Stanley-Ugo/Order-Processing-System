namespace OrderProcessingSystem.Domain.Events
{
    public record OrderPlacedEvent(Guid OrderId, Guid CustomerId, decimal TotalAmount, DateTime OccurredOn, List<OrderItemEvent> Items);
}
