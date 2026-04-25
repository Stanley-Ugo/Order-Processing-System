namespace OrderProcessingSystem.Application.Orders.DTOs
{
    public record OrderDto(
        Guid Id,
        Guid CustomerId,
        string Status,
        decimal TotalAmount,
        DateTime CreatedAt,
        List<OrderItemDto> Items
    );
}
