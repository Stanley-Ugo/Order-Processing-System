namespace OrderProcessingSystem.Application.Products.DTOs
{
    public record ProductDto(
        Guid Id,
        string Sku,
        string Name,
        decimal UnitPrice,
        int Stock
    );
}
