namespace OrderProcessingSystem.Application.Orders.DTOs
{
    public record OrderItemDto(
        Guid ProductId,
        string Sku,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal LineTotal
    );
}
