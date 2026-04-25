using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Domain.Entities;

namespace OrderProcessingSystem.Infrastructure.Persistence
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync()) return; // Already seeded

            var products = new[]
            {
            new Product
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Sku = "TSHIRT-RED-L",
                Name = "Red T-Shirt Large",
                UnitPrice = 25.99m,
                Stock = 100
            },
            new Product
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Sku = "TSHIRT-BLUE-M",
                Name = "Blue T-Shirt Medium",
                UnitPrice = 24.99m,
                Stock = 50
            },
            new Product
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Sku = "HOODIE-BLACK-XL",
                Name = "Black Hoodie XL",
                UnitPrice = 49.99m,
                Stock = 25
            }
        };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}
