using MediatR;
using OrderProcessingSystem.Application.Common;
using OrderProcessingSystem.Application.Products.DTOs;

namespace OrderProcessingSystem.Application.Products.Queries.GetProductBySku
{
    public record GetProductBySkuQuery(string Sku) : IRequest<Result<ProductDto>>;
}
