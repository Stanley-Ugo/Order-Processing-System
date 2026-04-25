namespace OrderProcessingSystem.Domain.Events
{
    public record OrderItemEvent(Guid ProductId, string Sku, int Quantity, decimal UnitPrice);
}
