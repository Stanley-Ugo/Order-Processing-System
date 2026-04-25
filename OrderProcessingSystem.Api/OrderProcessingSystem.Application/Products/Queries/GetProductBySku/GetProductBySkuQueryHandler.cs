using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Common;
using OrderProcessingSystem.Application.Common.Interfaces;
using OrderProcessingSystem.Application.Products.DTOs;

namespace OrderProcessingSystem.Application.Products.Queries.GetProductBySku
{
    public class GetProductBySkuQueryHandler : IRequestHandler<GetProductBySkuQuery, Result<ProductDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetProductBySkuQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<Result<ProductDto>> Handle(GetProductBySkuQuery request, CancellationToken ct)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Sku == request.Sku, ct);

            if (product == null)
                return Result<ProductDto>.Failure($"Product with SKU '{request.Sku}' not found");

            var dto = new ProductDto(
                product.Id,
                product.Sku,
                product.Name,
                product.UnitPrice,
                product.Stock
            );

            return Result<ProductDto>.Success(dto);
        }
    }
}
